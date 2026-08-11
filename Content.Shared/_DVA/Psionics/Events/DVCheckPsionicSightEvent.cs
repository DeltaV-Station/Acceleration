namespace Content.Shared._DVA.Psionics.Events;

[ByRefEvent]
public record struct DVCheckPsionicSightEvent(EntityUid User, bool CanSeePsionicInvisible = false);
