using Robust.Shared.Audio;

namespace Content.Server.Medical.CrewMonitoring;

/// <summary>
/// Extends upstream's CrewMonitoringConsoleComponent.
/// </summary>
public sealed partial class CrewMonitoringConsoleComponent : Component
{
    /// <summary>
    /// Whether or not alerts are enabled.
    /// </summary>
    [DataField]
    public bool AlertsEnabled = false;

    /// <summary>
    /// If an alert goes off, alerts are queued up until the cooldown is up.
    /// </summary>
    [DataField]
    public TimeSpan AlertCooldown = TimeSpan.FromSeconds(10);

    /// <summary>
    /// A set of sensors that have already fired an alert. Used to prevent the monitor from
    /// sounding an alert again after the cooldown is up if the person was dead/crit before.
    /// </summary>
    [DataField]
    public HashSet<string> AlertedSensors = [];

    /// <summary>
    /// The last time an alert was fired for someone goes critical.
    /// </summary>
    [DataField]
    public TimeSpan LastAlert = TimeSpan.Zero;

    /// <summary>
    /// The sound played when someone goes critical.
    /// </summary>
    [DataField]
    public SoundSpecifier AlertSound = new SoundPathSpecifier("/Audio/_DVA/Medical/CrewMonitoring/crew_alert.ogg");
}
