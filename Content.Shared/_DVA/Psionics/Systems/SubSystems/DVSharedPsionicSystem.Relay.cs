using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared._DVA.Psionics.Events.PowerActionEvents;
using Content.Shared.Mind;

namespace Content.Shared._DVA.Psionics.Systems;

public abstract partial class DVSharedPsionicSystem
{
    private void InitializeRelay()
    {
        SubscribeLocalEvent<DVPsionicPowersContainerComponent, MindRelayedEvent<DVDispelPowerActionEvent>>(RefRelayToPsionicPowersEvent);
        SubscribeLocalEvent<DVPsionicPowersContainerComponent, MindRelayedEvent<DVPsionicMindBrokenEvent>>(RefRelayToPsionicPowersEvent);
        SubscribeLocalEvent<DVPsionicPowersContainerComponent, DVGetPsionicPowerEntitiesEvent>(RefRelayToPsionicPowersEvent);
    }

    private void RefRelayToPsionicPowersEvent<T>(Entity<DVPsionicPowersContainerComponent> psionic, ref T args) where T : struct
    {
        RelayEvent(psionic, ref args);
    }

    private void RelayToPsionicPowersEvent<T>(Entity<DVPsionicPowersContainerComponent> psionic, T args) where T : class
    {
        RelayEvent(psionic, args);
    }

    private void RelayEvent<T>(Entity<DVPsionicPowersContainerComponent> psionic, ref T args) where T : struct
    {
        // this copies the by-ref event if it is a struct
        var ev = new DVPsionicPowerRelayedEvent<T>(args);
        foreach (var psionicPower in psionic.Comp.PsionicPowersContainer?.ContainedEntities ?? [])
        {
            RaiseLocalEvent(psionicPower, ref ev);
        }
        // and now we copy it back
        args = ev.Args;
    }

    private void RelayEvent<T>(Entity<DVPsionicPowersContainerComponent> psionic, T args) where T : class
    {
        var ev = new DVPsionicPowerRelayedEvent<T>(args);
        foreach (var psionicPower in psionic.Comp.PsionicPowersContainer?.ContainedEntities ?? [])
        {
            RaiseLocalEvent(psionicPower, ref ev);
        }
    }
}

/// <summary>
/// Event wrapper for relayed events.
/// </summary>
[ByRefEvent]
public record struct DVPsionicPowerRelayedEvent<TEvent>(TEvent Args);
