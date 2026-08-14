using Robust.Shared.GameStates;

namespace Content.Shared._DVA.Psionics.Components;

/// <summary>
/// Entities with this component cannot use psionic powers.
/// </summary>
/// <remarks>This should solely be used for StatusEffects. For insulative gear, see <see cref="DVPsionicallyInsulativeComponent"/></remarks>
[RegisterComponent, NetworkedComponent]
public sealed partial class DVPsionicsDisabledComponent : Component;
