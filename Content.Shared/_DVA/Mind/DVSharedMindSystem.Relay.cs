using Content.Shared._DVA.Psionics.Events;
using Content.Shared.Mind.Components;

namespace Content.Shared.Mind;

/// <summary>
/// The DeltaV partial System for relaying events to the mind.
/// </summary>
public abstract partial class SharedMindSystem
{
    public void InitializeDVRelay()
    {
        SubscribeLocalEvent<MindContainerComponent, DVDispelledEvent>(RelayRefToMind);
        SubscribeLocalEvent<MindContainerComponent, DVPsionicMindBrokenEvent>(RelayRefToMind);
    }
}
