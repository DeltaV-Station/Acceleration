using Robust.Shared.Audio;

namespace Content.Server._DVA.Medical.CrewMonitoring;

/// <summary>
/// Component used for alerts for the crew monitor.
/// 
/// Requires CrewMonitoringConsoleComponent or this won't do anything.
/// </summary>
[RegisterComponent]
[Access(typeof(DVCrewMonitorAlertsSystem))]
public sealed partial class DVCrewMonitorAlertsComponent : Component
{
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
