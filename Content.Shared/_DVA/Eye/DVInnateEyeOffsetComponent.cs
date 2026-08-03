using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared._DVA.Eye;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true), Access(typeof(DVSharedInnateEyeOffsetSystem))]
public sealed partial class DVInnateEyeOffsetComponent : Component
{
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadOnly)]
    public bool Active;
}

public sealed partial class DVToggleInnateEyeOffsetEvent : InstantActionEvent;
