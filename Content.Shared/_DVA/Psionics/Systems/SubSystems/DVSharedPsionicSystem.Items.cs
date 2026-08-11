using System.Linq;
using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared.Damage.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.StatusEffectNew;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._DVA.Psionics.Systems;

public abstract partial class DVSharedPsionicSystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] protected SharedAudioSystem Audio = default!;
    [Dependency] private StatusEffectsSystem _statusEffects = default!;

    [SubscribeLocalEvent]
    private void OnInsulativeGearEquipped(Entity<DVPsionicallyInsulativeComponent> gear, ref GotEquippedEvent args)
    {
        if (_timing.ApplyingState)
            return;

        if (!gear.Comp.AllowsPsionicUsage)
        {
            var ev = new DVPsionicSuppressedEvent(args.EquipTarget);
            RaiseLocalEvent(args.EquipTarget, ref ev);
        }
        if (gear.Comp.ShieldsFromPsionics)
        {
            var ev = new DVPsionicShieldedEvent(args.EquipTarget);
            RaiseLocalEvent(args.EquipTarget, ref ev);
        }
    }

    [SubscribeLocalEvent]
    private void OnInsulativeGearUnequipped(Entity<DVPsionicallyInsulativeComponent> gear, ref GotUnequippedEvent args)
    {
        if (_timing.ApplyingState)
            return;

        if (!gear.Comp.AllowsPsionicUsage && CanUsePsionicPower(args.EquipTarget))
        {
            var ev = new DVPsionicSuppressionStoppedEvent(args.EquipTarget);
            RaiseLocalEvent(args.EquipTarget, ref ev);
        }
        if (gear.Comp.ShieldsFromPsionics && CanBeTargeted(args.EquipTarget, showPopup: false))
        {
            var ev = new DVPsionicShieldingStoppedEvent(args.EquipTarget);
            RaiseLocalEvent(args.EquipTarget, ref ev);
        }
    }

    #region EventHandling
    [SubscribeLocalEvent]
    private void OnPowerUseAttempt(Entity<DVPsionicallyInsulativeComponent> gear, ref InventoryRelayedEvent<DVPsionicPowerUseAttemptEvent> args)
    {
        // If one gear blocks psionic usage, psionics cannot be used.
        args.Args.CanUsePower &= gear.Comp.AllowsPsionicUsage;
    }

    [SubscribeLocalEvent]
    private void OnTargetedByPsionicPower(Entity<DVPsionicallyInsulativeComponent> gear, ref InventoryRelayedEvent<DVTargetedByPsionicPowerEvent> args)
    {
        // If one gear shields from psionics, they're shielded.
        args.Args.IsShielded |= gear.Comp.ShieldsFromPsionics;
    }
    #endregion

    #region AntiPsionicWeaponry
    [SubscribeLocalEvent]
    private void OnAntiPsionicMeleeHit(Entity<DVAntiPsionicWeaponComponent> weapon, ref MeleeHitEvent args)
    {
        foreach (var target in args.HitEntities)
        {
            if (HasComp<DVPsionicComponent>(target))
            {
                Audio.PlayPredicted(weapon.Comp.HitSound, target, args.User);
                args.ModifiersList.Add(weapon.Comp.Modifiers);

                if (Random.Prob(weapon.Comp.DisableChance))
                    _statusEffects.TryUpdateStatusEffectDuration(target, PsionicsDisabledProtoId, TimeSpan.FromSeconds(10));
            }

            if (!weapon.Comp.Punish
                || !PotentialQuery.HasComp(target)
                || PsionicQuery.HasComp(target)
                || !Random.Prob(weapon.Comp.PunishChance))
                continue;

            _stuttering.DoStutter(args.User, TimeSpan.FromMinutes(5), false);
            _stun.TryKnockdown(args.User, TimeSpan.FromSeconds(5), false, drop: false);
            _jittering.DoJitter(args.User, TimeSpan.FromSeconds(5), false);
        }
    }

    [SubscribeLocalEvent]
    private void OnAntiPsionicStamHit(Entity<DVAntiPsionicWeaponComponent> weapon, ref StaminaMeleeHitEvent args)
    {
        if (args.HitList.Any(targetStamina => HasComp<DVPsionicComponent>(targetStamina.Entity)))
        {
            args.Multiplier *= weapon.Comp.StaminaDamageMultiplier;
        }
    }
    #endregion
}
