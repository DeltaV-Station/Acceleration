using Content.Shared.Inventory;

namespace Content.Shared._DVA.Psionics.Events;

public sealed class DVDispelledEvent(EntityUid dispeller, EntityUid target) : HandledEntityEventArgs, IInventoryRelayEvent
{
    /// <summary>
    /// The person who used dispel on the target.
    /// </summary>
    public EntityUid Dispeller = dispeller;

    /// <summary>
    /// The entity being dispelled.
    /// </summary>
    public EntityUid Target = target;

    public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;
}
