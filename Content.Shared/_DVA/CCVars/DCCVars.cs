using Robust.Shared.Configuration;

namespace Content.Shared._DVA.CCVars;

/// <summary>
/// DeltaV specific cvars.
/// </summary>
[CVarDefs]
// ReSharper disable once InconsistentNaming - Shush you
public sealed partial class DCCVars
{
    /*
     * Auto ACO
     */

    /// <summary>
    /// How long after the announcement before the spare ID is unlocked
    /// </summary>
    public static readonly CVarDef<TimeSpan> SpareIdUnlockDelay =
        CVarDef.Create("game.spare_id.unlock_delay", TimeSpan.FromMinutes(5), CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// How long to wait before checking for a captain after roundstart
    /// </summary>
    public static readonly CVarDef<TimeSpan> SpareIdAlertDelay =
        CVarDef.Create("game.spare_id.alert_delay", TimeSpan.FromMinutes(15), CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// Determines if the automatic spare ID process should automatically unlock the cabinet
    /// </summary>
    public static readonly CVarDef<bool> SpareIdAutoUnlock =
        CVarDef.Create("game.spare_id.auto_unlock", true, CVar.SERVERONLY | CVar.ARCHIVE);

    /*
     * Misc.
     */

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
}
