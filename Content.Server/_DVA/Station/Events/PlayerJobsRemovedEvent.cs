using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Content.Shared.Roles;

namespace Content.Server._DVA.Station.Events;

/// <summary>
/// Event is raised when a player loses jobs.
/// </summary>
[ByRefEvent]
public record struct PlayerJobsRemovedEvent(NetUserId Player, List<ProtoId<JobPrototype>> PlayerJobs);