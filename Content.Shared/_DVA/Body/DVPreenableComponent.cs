using Content.Shared.DoAfter;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._DVA.Body;

/// <summary>
/// Allows an entity to be 'preened', spawning a 'feather' from a regenerating supply
/// </summary>
/// <seealso cref="DVFeatherVisuals" />
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause, Access(typeof(DVPreenableSystem))]
public sealed partial class DVPreenableComponent : Component
{
    /// <summary>
    /// The entity to spawn on preening.
    /// </summary>
    [DataField]
    public EntProtoId FeatherPrototype;

    /// <summary>
    /// How many feathers the entity has to preen at the current time.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int CurrentFeathers = 3;

    /// <summary>
    /// The maximum amount of feathers this entity can preen.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int MaximumFeathers = 3;

    /// <summary>
    /// Time between a feather being depleted and when it'll regenerate.
    /// </summary>
    [DataField]
    public TimeSpan RegenerationDelay = TimeSpan.FromSeconds(150);

    /// <summary>
    /// When the next feather will regenerate.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan? RegenerateAt;

    /// <summary>
    /// Do-after duration for the preening verb.
    /// </summary>
    [DataField]
    public TimeSpan PreeningDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Verb text for the preening verb.
    /// </summary>
    [DataField]
    public LocId VerbText = "preening-verb";

    /// <summary>
    /// Displayed to the user when they preen themselves.
    /// </summary>
    [DataField]
    public LocId SelfPopup = "preening-popup.self";

    /// <summary>
    /// Displayed to the recipient when being preened.
    /// </summary>
    [DataField]
    public LocId RecipientPopup = "preening-popup.recipient";

    /// <summary>
    /// Displayed to the user when preening someone else.
    /// </summary>
    [DataField]
    public LocId UserPopup = "preening-popup.user";
}

/// <summary>
/// Event raised after completing the preening verb's do-after.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class PreeningDoAfterEvent : SimpleDoAfterEvent;

/// <summary>
/// Visual data set by <see cref="DVPreenableSystem" />. All colours.
/// </summary>
[Serializable, NetSerializable]
public enum DVFeatherVisuals : byte
{
    FeatherColor,
    BloodColor,
}
