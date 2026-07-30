using System.Linq;
using Content.Server.Medical.CrewMonitoring;
using Content.Server.Power.EntitySystems;
using Content.Shared.DeviceNetwork.Events;
using Content.Shared.Medical.SuitSensor;
using Content.Shared.PowerCell;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Server._DVA.Medical.CrewMonitoring;

/// <summary>
/// Used to process crew monitor sound alerts.
/// </summary>
public sealed partial class DVCrewMonitorAlertsSystem : EntitySystem
{
    [Dependency] private PowerCellSystem _cell = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        // Needs to be ordered so that the coordinates from CrewMonitoringConsoleComponent are up to date
        SubscribeLocalEvent<DVCrewMonitorAlertsComponent, DeviceNetworkPacketEvent>(OnPacketReceived, after: [typeof(CrewMonitoringConsoleSystem)]);
    }

    private void OnPacketReceived(Entity<DVCrewMonitorAlertsComponent> ent, ref DeviceNetworkPacketEvent args)
    {
        // Check if it has power
        if (!this.IsPowered(ent.Owner, EntityManager) || !_cell.HasActivatableCharge(ent.Owner))
            return;

        // check if its a crew monitor (we need the sensors)
        if (!TryComp<CrewMonitoringConsoleComponent>(ent, out var crewMonitor))
            return;

        var sensors = crewMonitor.ConnectedSensors;
        // Filter on dead or critical
        var alertingSensors = sensors
            .Where(sensor => IsCriticalOrDead(sensor.Value));

        // Check for any alerted sensors that should be cleared out (because they were healed)
        var staleAlerts = sensors
            .Except(alertingSensors) // Filter on people not critical/dead
            .Select(kvp => kvp.Key)
            .Intersect(ent.Comp.AlertedSensors); // Find "alerted" people that are healthy

        if (staleAlerts.Any())
            _ = ent.Comp.AlertedSensors.RemoveWhere(alert => staleAlerts.Contains(alert));

        // alert is still on cooldown, defer checking if we should alert
        if (ent.Comp.LastAlert + ent.Comp.AlertCooldown > _timing.CurTime)
            return;

        if (alertingSensors.Any())
        {
            // Look for new alerts and only alert if there are new crew to alert on
            var newAlerts = alertingSensors
                .Select(sensor => sensor.Key)
                .Except(ent.Comp.AlertedSensors); // Filter out crew who were already alerted on

            if (newAlerts.Any())
                Alert(ent);

            // Overwrite the alerted sensors with all alerting sensors for the next time alerts can fire
            ent.Comp.AlertedSensors = [.. alertingSensors.Select(sensor => sensor.Key)];
        }
    }

    private void Alert(Entity<DVCrewMonitorAlertsComponent> monitor)
    {
        var audioParams = AudioParams.Default.WithVolume(-2f).WithMaxDistance(4f);
        _ = _audio.PlayPvs(monitor.Comp.AlertSound, monitor.Owner, audioParams);
        monitor.Comp.LastAlert = _timing.CurTime;
    }

    private static bool IsCriticalOrDead(SuitSensorStatus status)
    {
        return !status.IsAlive || status.DamagePercentage >= 1f;
    }
}
