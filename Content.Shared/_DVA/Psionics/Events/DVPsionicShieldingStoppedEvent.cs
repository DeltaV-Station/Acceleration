namespace Content.Shared._DVA.Psionics.Events;

/// <summary>
/// This is raised on an entity when they are no longer psionically shielded through a status effect anymore.
/// </summary>
/// <param name="Shielded">The entity who is no longer psionically shielded.</param>
[ByRefEvent]
public record struct DVPsionicShieldingStoppedEvent(EntityUid Shielded);
