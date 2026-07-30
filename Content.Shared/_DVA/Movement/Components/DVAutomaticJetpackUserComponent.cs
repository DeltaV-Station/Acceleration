using Robust.Shared.GameStates;

namespace Content.Shared._DVA.Movement.Components;

/// <summary>
/// Added when someone holds a jetpack that is toggled active and waits to turn on.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DVAutomaticJetpackUserComponent : Component
{
    /// <summary>
    /// The jetpack that will automatically activate.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid Jetpack;
}
