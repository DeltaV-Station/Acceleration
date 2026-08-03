using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DVA.Body;

/// <summary>
/// Causes an entity to preen when they take a sufficient amount of damage.
/// </summary>
/// <seealso cref="DVPreenableComponent" />
[RegisterComponent, NetworkedComponent]
public sealed partial class DVPreenOnDamagedComponent : Component
{
    /// <summary>
    /// The feather prototype to spawn when damaged.
    /// </summary>
    [DataField]
    public EntProtoId FeatherPrototype;

    /// <summary>
    /// Incoming damage types to consider.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<DamageTypePrototype>> DamageTypes = new()
    {
        "Blunt",
        "Piercing",
        "Slash",
    };

    /// <summary>
    /// Minimum damage to be dealt to consider preening on damage.
    /// </summary>
    [DataField]
    public FixedPoint2 DamageThreshold = 9;

    /// <summary>
    /// Probability per each unit of total damage to preen.
    /// </summary>
    [DataField]
    public float ChancePerDamagePoint = 0.0125f;

    /// <summary>
    /// Displayed to the entity when they preen from damage.
    /// </summary>
    [DataField]
    public LocId DroppedPopup = "preen-on-damaged-feather-dropped";

    /// <summary>
    /// Emote to play from the entity when they preen from damage.
    /// </summary>
    [DataField]
    public ProtoId<EmotePrototype> ScreamEmote = "Scream";

    /// <summary>
    /// Solution to inject into the entity when they preen from damage.
    /// </summary>
    [DataField]
    public Solution AdrenalineSolution = new([new("Epinephrine", 5)]);
}
