using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Prototypes;

namespace Content.Shared._DVA.Psionics.Systems;

public abstract partial class DVSharedPsionicSystem
{
    public static readonly EntProtoId PsionicsDisabledProtoId = "StatusEffectPsionicsDisabled";

    [SubscribeLocalEvent]
    private void OnPowerUseAttempt(Entity<DVPsionicsDisabledComponent> psionic, ref StatusEffectRelayedEvent<DVPsionicPowerUseAttemptEvent> args)
    {
        var ev = args.Args;
        ev.CanUsePower = false;
        args.Args = ev;
    }

    [SubscribeLocalEvent]
    private void OnTargetedByPsionicPower(Entity<DVShieldedFromPsionicsComponent> psionic, ref StatusEffectRelayedEvent<DVTargetedByPsionicPowerEvent> args)
    {
        var ev = args.Args;
        ev.IsShielded = true;
        args.Args = ev;
    }

    [SubscribeLocalEvent]
    private void OnStatusEffectApplied(Entity<DVPsionicsDisabledComponent> statusEffect, ref StatusEffectAppliedEvent args)
    {
        var ev = new DVPsionicSuppressedEvent(args.Target);
        RaiseLocalEvent(args.Target, ref ev);
    }

    [SubscribeLocalEvent]
    private void OnStatusEffectRemoved(Entity<DVPsionicsDisabledComponent> statusEffect, ref StatusEffectRemovedEvent args)
    {
        if (!CanUsePsionicPower(args.Target))
            return;

        var ev = new DVPsionicSuppressionStoppedEvent(args.Target);
        RaiseLocalEvent(args.Target, ref ev);
    }

    [SubscribeLocalEvent]
    private void OnStatusEffectApplied(Entity<DVShieldedFromPsionicsComponent> statusEffect, ref StatusEffectAppliedEvent args)
    {
        var ev = new DVPsionicShieldedEvent(args.Target);
        RaiseLocalEvent(args.Target, ref ev);
    }

    [SubscribeLocalEvent]
    private void OnStatusEffectRemoved(Entity<DVShieldedFromPsionicsComponent> statusEffect, ref StatusEffectRemovedEvent args)
    {
        if (!CanBeTargeted(args.Target, showPopup: false))
            return;

        var ev = new DVPsionicShieldingStoppedEvent(args.Target);
        RaiseLocalEvent(args.Target, ref ev);
    }
}
