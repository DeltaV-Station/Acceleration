namespace Content.Shared._DVA.Psionics.Events;

[ByRefEvent]
public record struct DVGetPsionicPowerEntitiesEvent()
{
    public List<EntityUid> PsionicPowerEntities = [];
}
