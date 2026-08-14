using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Systems;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Shared._DVA.EntityEffects.Effects.Psionics;

/// <summary>
/// This entity effect will remove all psionic powers from the entity - Unless they cannot be removed.
/// </summary>
/// <inheritdoc cref="EntityEffect"/>
public sealed partial class RemovePsionicPowersEntityEffectSystem : EntityEffectSystem<DVPotentialPsionicComponent, RemovePsionicPowers>
{
    [Dependency] private DVSharedPsionicSystem _psionic = default!;
    protected override void Effect(Entity<DVPotentialPsionicComponent> entity, ref EntityEffectEvent<RemovePsionicPowers> args)
    {
        _psionic.MindBreakEntity(entity);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class RemovePsionicPowers : EntityEffectBase<RemovePsionicPowers>
{
    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-chem-remove-psionic", ("chance", Probability));
    }
}
