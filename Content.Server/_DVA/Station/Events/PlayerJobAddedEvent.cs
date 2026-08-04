using Robust.Shared.Network;

namespace Content.Server._DVA.Station.Events;

/// <summary>
/// Event is raised when a player takes a job.
/// </summary>
[ByRefEvent]
public record struct PlayerJobAddedEvent(NetUserId Player, string JobPrototypeId);