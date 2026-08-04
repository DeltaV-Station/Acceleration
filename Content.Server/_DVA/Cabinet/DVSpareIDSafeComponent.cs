namespace Content.Server._DVA.Cabinet;

/// <summary>
///  Component that marks an entity as a spare ID safe. Is interacted with by DVAutomaticSpareIdSystem to unlock the safe when there is no captain present.
/// </summary>
[RegisterComponent]
public sealed partial class DVSpareIDSafeComponent : Component;