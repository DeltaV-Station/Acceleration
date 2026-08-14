namespace Content.Shared._DVA.Psionics.Events;

/// <summary>
/// Raised on an entity when they are no longer psionically suppressed from using psionic abilities.
/// </summary>
/// <param name="Victim">The entity who is psionically suppressed</param>
[ByRefEvent]
public record struct DVPsionicSuppressedEvent(EntityUid Victim);
