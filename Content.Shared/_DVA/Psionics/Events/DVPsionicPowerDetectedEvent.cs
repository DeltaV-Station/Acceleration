namespace Content.Shared._DVA.Psionics.Events;

/// <summary>
/// Event raised on an entity that can detect when someone used a psionic power nearby.
/// </summary>
/// <param name="Psionic">The psionic that detected the psionic usage.</param>
/// <param name="Power">The psionic power that was used.</param>
[ByRefEvent]
public readonly record struct DVPsionicPowerDetectedEvent(EntityUid Psionic, string Power);
