using Content.Shared._DVA.Psionics.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared._DVA.Psionics.Components;

/// <summary>
/// Describes a container for psionic powers that are present on the mind entity.
/// Is applied automatically upon adding any psionic power.
/// Can be used for tracking currently gained psionic powers.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(DVSharedPsionicSystem))]
public sealed partial class DVPsionicPowersContainerComponent : Component
{
    public const string ContainerId = "psionic-powers";

    /// <summary>
    /// The actual container holding references to the gained psionic powers.
    /// </summary>
    [ViewVariables]
    public Container? PsionicPowersContainer;

    /// <summary>
    /// The entity that is using the powers.
    /// This component is stored on the mind, so this is the entity controlled by said mind.
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public EntityUid? AttachedEntity;
}
