using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._DVA.Traits;

/// <summary>
/// Sent from server to client when a player spawns with traits that were disabled due to unmet conditions.
/// </summary>
[Serializable, NetSerializable]
public sealed class DisabledTraitsEvent(Dictionary<ProtoId<DVTraitPrototype>, List<string>> disabledTraits)
    : EntityEventArgs
{
    /// <summary>
    /// Dictionary mapping disabled trait IDs to lists of reasons why they were disabled.
    /// </summary>
    public Dictionary<ProtoId<DVTraitPrototype>, List<string>> DisabledTraits = disabledTraits;
}
