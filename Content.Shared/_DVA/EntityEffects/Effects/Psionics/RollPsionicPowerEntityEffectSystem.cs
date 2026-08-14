using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Systems;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Shared._DVA.EntityEffects.Effects.Psionics;

/// <summary>
/// attempts to roll for a new psionic power.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T, TEffect}"/>
public sealed partial class RollPsionicPowerEntityEffectSystem : EntityEffectSystem<DVPotentialPsionicComponent, RollPsionicPower>
{
    [Dependency] private DVSharedPsionicSystem _psionic = default!;

    protected override void Effect(Entity<DVPotentialPsionicComponent> psionic, ref EntityEffectEvent<RollPsionicPower> args)
    {
        _psionic.TryRollPsionic(psionic, args.Effect.BonusMultiplier);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class RollPsionicPower : EntityEffectBase<RollPsionicPower>
{
    /// <summary>
    /// Chance multiplier.
    /// </summary>
    [DataField]
    public float BonusMultiplier = 1f;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-chem-roll-psionic", ("multiplier", BonusMultiplier));
    }
}
