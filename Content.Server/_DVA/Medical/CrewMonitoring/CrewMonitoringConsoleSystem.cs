using System.Linq;
using Content.Server.Power.EntitySystems;
using Content.Shared.Medical.SuitSensor;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Server.Medical.CrewMonitoring;

/// <summary>
/// Extends upstream's CrewMonitoringConsoleSystem.
/// </summary>
public sealed partial class CrewMonitoringConsoleSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private void ProcessAlerts(Entity<CrewMonitoringConsoleComponent> monitor, Dictionary<string, SuitSensorStatus> sensors)
    {
        // Alerts disabled
        if (!monitor.Comp.AlertsEnabled)
            return;

        // Check if it has power
        if (!this.IsPowered(monitor.Owner, EntityManager) || !_cell.HasActivatableCharge(monitor.Owner))
            return;

        // alert is still on cooldown, defer processing alerts
        if (monitor.Comp.LastAlert + monitor.Comp.AlertCooldown > _timing.CurTime)
            return;

        // Filter on dead or critical
        var alertingSensors = sensors
            .Where(sensor => IsCriticalOrDead(sensor.Value));

        if (alertingSensors.Any())
        {
            // Look for new alerts
            var newAlerts = alertingSensors
                .Select(sensor => sensor.Key)
                .Except(monitor.Comp.AlertedSensors); // Filter out people who were already alerted on

            if (newAlerts.Any())
                Alert(monitor);

            // Overwrite the alerted sensors with all alerting sensors for the next time alerts can fire
            monitor.Comp.AlertedSensors = [.. alertingSensors.Select(sensor => sensor.Key)];
        }

        // Check for any alerted sensors that should be cleared out (because they were healed)
        var staleAlerts = sensors
            .Except(alertingSensors) // Filter on people not critical/dead
            .Select(kvp => kvp.Key)
            .Intersect(monitor.Comp.AlertedSensors); // Find "alerted" people that are healthy

        if (staleAlerts.Any())
            monitor.Comp.AlertedSensors.RemoveWhere(alert => staleAlerts.Contains(alert));
    }

    private void Alert(Entity<CrewMonitoringConsoleComponent> monitor)
    {
        var audioParams = AudioParams.Default.WithVolume(-2f).WithMaxDistance(4f);
        _audio.PlayPvs(monitor.Comp.AlertSound, monitor.Owner, audioParams);
        monitor.Comp.LastAlert = _timing.CurTime;
    }

    private bool IsCriticalOrDead(SuitSensorStatus status)
    {
        return !status.IsAlive || status.DamagePercentage >= 1f;
    }
}