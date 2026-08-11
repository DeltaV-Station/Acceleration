using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Systems;
using Content.Shared.GameTicking;

namespace Content.Server._DVA.Psionics.Systems;

public sealed partial class DVPsionicSystem : DVSharedPsionicSystem
{
    [SubscribeLocalEvent]
    private void OnPlayerSpawnComplete(Entity<DVPotentialPsionicComponent> potPsionic, ref PlayerSpawnCompleteEvent args)
    {
        if (RollChance(potPsionic))
            AddRandomPsionicPower(potPsionic, false);
    }
}
