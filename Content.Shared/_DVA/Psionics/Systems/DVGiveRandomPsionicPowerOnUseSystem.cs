using Content.Shared._DVA.Psionics.Components;
using Content.Shared.Interaction.Events;

namespace Content.Shared._DVA.Psionics.Systems;

public sealed partial class DVGiveRandomPsionicPowerOnUseSystem : EntitySystem
{
    [Dependency] private DVSharedPsionicSystem _psionic = default!;

    [SubscribeLocalEvent]
    private void OnUse(Entity<DVGiveRandomPsionicPowerOnUseComponent> item, ref UseInHandEvent args)
    {
        if (!TryComp<DVPotentialPsionicComponent>(args.User, out var potPsionic))
            return;

        _psionic.TryRollPsionic((args.User, potPsionic), 10f);
        PredictedQueueDel(item);
    }
}
