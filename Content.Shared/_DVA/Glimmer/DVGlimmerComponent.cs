using Content.Shared._DVA.Utility;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._DVA.Glimmer;

/// <summary>
/// Component tracking glimmer for a round. Usually on a singleton entity.
/// </summary>
/// <seealso cref="DVGlimmerSystem" />
[RegisterComponent, NetworkedComponent]
[Access(typeof(DVGlimmerSystem))]
public sealed partial class DVGlimmerComponent : Component
{
    /// <summary>
    /// The current level of glimmer.
    /// </summary>
    [DataField]
    public int Glimmer;

    /// <summary>
    /// The minimum level of glimmer.
    /// </summary>
    [DataField]
    public int MinGlimmer;

    /// <summary>
    /// The maximum level of glimmer.
    /// </summary>
    [DataField]
    public int MaxGlimmer = 1000;

    /// <summary>
    /// The tier definitions of glimmer.
    /// </summary>
    [DataField]
    public SortedDictionary<int, GlimmerTier> Tiers = new()
    {
        { 0, GlimmerTier.Minimal },
        { 50, GlimmerTier.Low },
        { 100, GlimmerTier.Moderate },
        { 300, GlimmerTier.High },
        { 500, GlimmerTier.Dangerous },
        { 900, GlimmerTier.Critical },
    };

    /// <summary>
    /// The current tier of glimmer per <see cref="Glimmer" /> and <see cref="Tiers" />
    /// </summary>
    [ViewVariables]
    public GlimmerTier Tier => Tiers.HighestMatch(Glimmer) ?? GlimmerTier.Minimal;
}

[Serializable, NetSerializable]
public enum GlimmerTier : byte
{
    Minimal,
    Low,
    Moderate,
    High,
    Dangerous,
    Critical,
}

/// <summary>
/// Raised when the glimmer changes actively (i.e. due to game logic)
/// and should potentially result in more game logic happening
/// </summary>
[ByRefEvent]
public readonly record struct GlimmerChangedActiveEvent(Entity<DVGlimmerComponent> Depth);

/// <summary>
/// Raised when the glimmer changes passively (i.e. due to networking)
/// and should only result in cosmetic changes
/// </summary>
[ByRefEvent]
public readonly record struct GlimmerChangedPassiveEvent(Entity<DVGlimmerComponent> Depth);
