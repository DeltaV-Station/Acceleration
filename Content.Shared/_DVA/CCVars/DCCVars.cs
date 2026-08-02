using Robust.Shared.Configuration;

namespace Content.Shared._DVA.CCVars;

/// <summary>
/// DeltaV specific cvars.
/// </summary>
[CVarDefs]
// ReSharper disable once InconsistentNaming - Shush you
public sealed partial class DCCVars
{
    /// <summary>
    /// The total time a player has to be SSD to be considered cryoable (stage 3).
    /// Default is 20 minutes. Value should be bigger than <see cref="SsdIndicatorRecentAfterSeconds"/>.
    /// </summary>
    public static readonly CVarDef<float> SsdIndicatorCryoableAfterSeconds =
        CVarDef.Create("deltav.ssd.cryoable_after_seconds", 1200f, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// The total time a player has to be SSD to be considered recently SSD (stage 2).
    /// If the player has been SSD for less than this time, they are considered "very recently" SSD (stage 1).
    /// Default is 5 minutes. Value should be smaller than <see cref="SsdIndicatorCryoableAfterSeconds"/>.
    /// </summary>
    public static readonly CVarDef<float> SsdIndicatorRecentAfterSeconds =
        CVarDef.Create("deltav.ssd.recent_after_seconds", 300f, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    ///    Maximum number of characters in objective summaries.
    /// </summary>
    public static readonly CVarDef<int> MaxObjectiveSummaryLength =
        CVarDef.Create("game.max_objective_summary_length", 256, CVar.SERVER | CVar.REPLICATED);
}
