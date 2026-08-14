using Content.Shared.DoAfter;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DVA.Psionics.Components.Powers;

/// <summary>
/// Every psionic power entity has this component for storing generic data that any psionic power uses.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DVPsionicPowerComponent : Component
{
    /// <summary>
    /// The actual UID for the action entity. It'll be saved here when the component is initialized.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? ActionEntity;

    /// <summary>
    /// The prototype ID for the action.
    /// It's set up in the component, optionally overriden via YML and then referenced via a string here.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId ActionProtoId;

    /// <summary>
    /// The name of the power. It's used for logging and showing others what power was used.
    /// </summary>
    [DataField(required: true)]
    public string PowerName;

    /// <summary>
    /// The minimum glimmer amount that will be changed upon use of the psionic power.
    /// Should be lower than <see cref="MaxGlimmerChanged"/>.
    /// </summary>
    [DataField(required: true)]
    public int MinGlimmerChanged;

    /// <summary>
    /// The maximum glimmer amount that will be changed upon use of the psionic power.
    /// Should be higher than <see cref="MinGlimmerChanged"/>.
    /// </summary>
    [DataField(required: true)]
    public int MaxGlimmerChanged;

    /// <summary>
    /// Whether this ability can be removed via mindbreaking.
    /// </summary>
    /// <example>Revenants shouldn't be able to lose their powers.</example>
    [DataField]
    public bool CanBeRemoved = true;

    /// <summary>
    /// When a power uses a DoAfter, the ID can be saved here for convenience.
    /// It'll handle being dispelled automatically.
    /// It'll need to be broken up into the DoAfter EntityUid and ushort index first.
    /// </summary>
    [DataField, AutoNetworkedField]
    private EntityUid? _doAfterEntityId;

    /// <summary>
    /// When a power uses a DoAfter, the index can be saved here for convenience.
    /// It'll handle being dispelled automatically.
    /// It'll need to be broken up into the DoAfter EntityUid and ushort index first.
    /// </summary>
    [DataField, AutoNetworkedField]
    private ushort? _doAfterIdIndex;

    /// <summary>
    /// Helper method to save a DoAfterId as DoAfterIds are not serializable and therefore cannot be networked.
    /// It's parts can be though, and can be rebuilt.
    /// </summary>
    /// <param name="doAfterId">The DoAfterId to save. If null, it'll remove the saved DoAfterId.</param>
    public void SaveDoAfterId(DoAfterId doAfterId)
    {
        _doAfterEntityId = doAfterId.Uid;
        _doAfterIdIndex = doAfterId.Index;
    }

    /// <summary>
    /// Helper method to remove the saved DoAfterId.
    /// </summary>
    public void RemoveSavedDoAfterId()
    {
        _doAfterEntityId = null;
        _doAfterIdIndex = null;
    }

    /// <summary>
    /// A helper method to get a saved DoAfterId.
    /// </summary>
    /// <returns>Returns a DoAfterId if one is present, null if not.</returns>
    public DoAfterId? GetDoAfterId()
    {
        if (_doAfterEntityId is not { } doAfterId
            || _doAfterIdIndex is not { } doAfterIndex)
            return null;

        return new DoAfterId(doAfterId, doAfterIndex);
    }
}
