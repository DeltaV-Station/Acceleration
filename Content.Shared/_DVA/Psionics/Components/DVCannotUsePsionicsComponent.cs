using Robust.Shared.GameStates;

namespace Content.Shared._DVA.Psionics.Components;

/// <summary>
/// This prevents entities from getting actions or other benefits from their powers.
/// </summary>
/// <example>Brains shouldn't be able to use powers.</example>
[RegisterComponent, NetworkedComponent]
public sealed partial class DVCannotUsePsionicsComponent : Component;
