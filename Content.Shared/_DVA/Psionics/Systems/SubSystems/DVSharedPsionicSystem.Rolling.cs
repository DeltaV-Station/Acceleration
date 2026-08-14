using Content.Shared._DVA.Psionics.Components;
using Content.Shared.EntityTable;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Shared._DVA.Psionics.Systems;

public abstract partial class DVSharedPsionicSystem
{
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private EntityTableSystem _entityTable = default!;
    [Dependency] private SharedMindSystem _mind = default!;

    public bool TryRollPsionic(Entity<DVPotentialPsionicComponent> potPsionic, float multiplier = 1.0f)
    {
        if (potPsionic.Comp.Rolled)
            return false;

        potPsionic.Comp.Rolled = true;

        if (!RollChance(potPsionic, multiplier))
        {
            Popup.PopupEntity(Loc.GetString("psionic-roll-failed"), potPsionic, potPsionic, PopupType.Medium);
            return false;
        }

        AddRandomPsionicPower(potPsionic, true);
        Dirty(potPsionic);
        return true;
    }

    protected bool RollChance(Entity<DVPotentialPsionicComponent> potPsionic, float multiplier = 1.0f)
    {
        var chance = potPsionic.Comp.BaseChance;
        // Jobs like Command and Chaplains get a bonus on their roll.
        chance += potPsionic.Comp.JobBonusChance;
        // Species like Kitsunes get a bonus on their roll.
        chance += potPsionic.Comp.SpeciesBonusChance;

        // Rolling with chemicals can have multipliers.
        chance *= multiplier;

        chance = Math.Clamp(chance, 0, 1);
        return SharedRandomExtensions.PredictedProb(_timing, chance, GetNetEntity(potPsionic));
    }

    public void AddRandomPsionicPower(Entity<DVPotentialPsionicComponent> potPsionic, bool midRound)
    {
        if (!_prototypeManager.Resolve(potPsionic.Comp.PsionicPowerTableId, out var powerTable)
            || !_mind.TryGetMind(potPsionic.Owner, out var mind, out _))
            return;

        EnsureComp<DVPsionicPowersContainerComponent>(mind, out var container);
        container.AttachedEntity = potPsionic;
        var takenPsionics = container.PsionicPowersContainer;

        var attempts = 0;
        while (attempts < 20) // 20 attempts to get a unique psionic power.
        {
            var spawns = _entityTable.GetSpawns(powerTable, Random);

            foreach (var entProtoId in spawns)
            {
                if (TryAddPsionicPower(potPsionic, mind, takenPsionics, midRound, entProtoId))
                    return;

                attempts++;
            }
        }

        Popup.PopupEntity(Loc.GetString("psionic-roll-failed"), potPsionic, potPsionic, PopupType.Medium);
    }

    private bool TryAddPsionicPower(Entity<DVPotentialPsionicComponent> potPsionic, EntityUid mind, Container? takenPsionics, bool midRound, EntProtoId entProtoId)
    {
        foreach (var takenPsionic in takenPsionics?.ContainedEntities ?? [])
        {
            var meta = MetaData(takenPsionic);
            // If they have the power already, don't add it again.
            if (meta.EntityPrototype is not null && meta.EntityPrototype == entProtoId)
            {
                return false;
            }
        }

        if (!_prototypeManager.Resolve(entProtoId, out var powerEntity))
            return false;

        if (!PredictedTrySpawnInContainer(entProtoId,
                mind,
                DVPsionicPowersContainerComponent.ContainerId,
                out _))
            return false;

        if (!midRound)
            return true;
        // For alternative means of getting psionics that aren't via spawning in, cause them to suffer.
        _stuttering.DoStutter(potPsionic, TimeSpan.FromMinutes(1), false);
        _stun.TryKnockdown(potPsionic.Owner, TimeSpan.FromSeconds(3), false, drop: false);
        _jittering.DoJitter(potPsionic, TimeSpan.FromSeconds(3), false);

        return true;
    }

    public bool GrantPsionicRoll(Entity<DVPotentialPsionicComponent?> potPsionic)
    {
        if (!Resolve(potPsionic, ref potPsionic.Comp) || !potPsionic.Comp.Rolled)
            return false;

        potPsionic.Comp.Rolled = false;
        Dirty(potPsionic);
        return true;
    }
}
