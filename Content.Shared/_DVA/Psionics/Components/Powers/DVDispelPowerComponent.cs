using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DVA.Psionics.Components.Powers;

/// <summary>
/// The psionic power to dispel entities that have special dispellable behavior.
/// This also interrupts the active powers from other psionics and lets them see psionically invisible entities.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DVDispelPowerComponent : Component;
