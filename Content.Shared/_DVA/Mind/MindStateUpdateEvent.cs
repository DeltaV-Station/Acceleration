namespace Content.Shared._DVA.Mind;

using Content.Shared.Mind.Components;

[ByRefEvent]
public record struct MindStateUpdatedEvent(MindState State);
