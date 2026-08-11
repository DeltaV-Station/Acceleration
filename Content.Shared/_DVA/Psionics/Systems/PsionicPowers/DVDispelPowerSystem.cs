using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Components.Powers;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared._DVA.Psionics.Events.PowerActionEvents;
using Content.Shared.Bible.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Guardian.Components;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Revenant.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._DVA.Psionics.Systems.PsionicPowers;

/// <summary>
/// This enables psionic users to dispel noospheric beings and actions.
/// </summary>
public sealed partial class DVDispelPowerSystem : DVBasePsionicPowerSystem<DVDispelPowerComponent, DVDispelPowerActionEvent>
{
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private DamageableSystem _damageable = default!;
    [Dependency] private StatusEffectsSystem _statusEffects = default!;
    [Dependency] private EntityQuery<DVPsionicPowerComponent> _powerQuery = default!;

    [SubscribeLocalEvent]
    private void OnPowerMapInit(Entity<DVDispelPowerComponent> power, ref MapInitEvent args)
    {
        // Dispell psionics can now see invisible entities to dispell them.
        Psionic.SetCanSeePsionicInvisiblity(power.Owner, true);
    }

    protected override void OnPowerUsed(Entity<DVDispelPowerComponent> psionic, ref DVDispelPowerActionEvent args)
    {
        if (!Psionic.CanBeTargeted(args.Target, source: args.Performer))
            return;

        var ev = new DVDispelledEvent(args.Performer, args.Target);
        RaiseLocalEvent(args.Target, ev);
    }

    [SubscribeLocalEvent]
    private void OnPsionicallySuppressed(Entity<DVDispelPowerComponent> power, ref DVPsionicSuppressedEvent args)
    {
        // Don't let them see if they're suppressed.
        Psionic.SetCanSeePsionicInvisiblity(args.Victim, false);
    }

    [SubscribeLocalEvent]
    private void OnStoppedPsionicallySuppressed(Entity<DVDispelPowerComponent> psionic, ref DVPsionicSuppressionStoppedEvent args)
    {
        // Let mah people SEE
        Psionic.SetCanSeePsionicInvisiblity(args.Victim, true);
    }

    /// <summary>
    /// This is necessary to avoid dispel users to lose their invisibility sight.
    /// This fires after the System where losing psionic shielding makes you unable to see invisible entities.
    /// </summary>
    /// <param name="psionic">The psionic who lost their shielding.</param>
    /// <param name="args">The event.</param>
    [SubscribeLocalEvent]
    private void OnPsionicSightCheck(Entity<DVDispelPowerComponent> psionic, ref DVPsionicPowerRelayedEvent<MindRelayedEvent<DVCheckPsionicSightEvent>> args)
    {
        if (Psionic.CanUsePsionicPower(psionic))
            Psionic.SetCanSeePsionicInvisiblity(args.Args.Args.User, true);
    }

    [SubscribeLocalEvent]
    private void OnMindBroken(Entity<DVDispelPowerComponent> psionic, ref DVPsionicPowerRelayedEvent<MindRelayedEvent<DVPsionicMindBrokenEvent>> args)
    {
        if (!_powerQuery.TryComp(psionic, out var powerComp) || powerComp.CanBeRemoved)
            return;

        Psionic.SetCanSeePsionicInvisiblity(psionic, false);
    }

    [SubscribeLocalEvent]
    private void OnDeleteDispelled(Entity<DVDeleteOnDispellComponent> dispellable, ref DVDispelledEvent args)
    {
        PredictedQueueDel(dispellable);
        PredictedSpawnAtPosition("Ash", Transform(dispellable).Coordinates);
        Popup.PopupEntity(Loc.GetString("psionic-burns-up", ("item", dispellable)), dispellable, args.Dispeller, PopupType.MediumCaution);
        _audio.PlayPredicted(dispellable.Comp.DispelSound, dispellable.Owner, args.Dispeller);
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnDmgDispelled(Entity<DVDamageOnDispelComponent> damaged, ref DVDispelledEvent args)
    {
        var damage = damaged.Comp.Damage;
        var modifier = Random.NextFloat(damaged.Comp.Variance, 1 + damaged.Comp.Variance);

        damage *= modifier;
        DealDispelDamage(damaged, damage, args.Dispeller, damaged.Comp.DispelSound);
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnRevenantDispelled(Entity<RevenantComponent> revenant, ref DVDispelledEvent args)
    {
        DealDispelDamage(revenant, dispeller: args.Dispeller);
        // TODO: Port over the new StatusEffectSystem when upstream ports over the Corporeal status effect to the new system.
        _statusEffects.TryAddStatusEffect(revenant, "Corporeal", TimeSpan.FromSeconds(30), false, "Corporeal");
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnGuardianDispelled(Entity<GuardianComponent> guardian, ref DVDispelledEvent args)
    {
        DealDispelDamage(guardian, dispeller: args.Dispeller);
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnFamiliarDispelled(Entity<FamiliarComponent> familiar, ref DVDispelledEvent args)
    {
        if (familiar.Comp.Source != null)
            EnsureComp<SummonableRespawningComponent>(familiar.Comp.Source.Value);

        args.Handled = true;
    }

    private void DealDispelDamage(EntityUid dispelled, DamageSpecifier? damage = null, EntityUid? dispeller = null, SoundSpecifier? sound = null)
    {
        if (Deleted(dispelled))
            return;

        Popup.PopupEntity(Loc.GetString("psionic-burn-resist", ("item", dispelled)), dispelled, dispeller, PopupType.SmallCaution);
        _audio.PlayPredicted(sound, dispelled, dispeller);

        if (damage == null)
        {
            damage = new DamageSpecifier();
            damage.DamageDict.Add("Blunt", 100);
        }

        _damageable.TryChangeDamage(dispelled, damage, ignoreResistances: true);
    }
}
