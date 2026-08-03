using System.Linq;
using Content.Shared.Body.Systems;
using Content.Shared.Chat;
using Content.Shared.Damage.Systems;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._DVA.Body;

public sealed partial class DVPreenOnDamagedSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private DVPreenableSystem _preenable = default!;
    [Dependency] private SharedChatSystem _chat = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedPhysicsSystem _physics = default!;
    [Dependency] private SharedBloodstreamSystem _bloodstream = default!;

    private const float FeatherLaunchImpulse = 8;
    private const float FeatherLaunchImpulseVariance = 3;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DVPreenOnDamagedComponent, DamageDealtEvent>(OnDamageDealt);
    }

    private void OnDamageDealt(Entity<DVPreenOnDamagedComponent> ent, ref DamageDealtEvent args)
    {
         var totalApplicableDamage = args.Damage.DamageDict
            .Where(kvp => ent.Comp.DamageTypes.Contains(kvp.Key))
            .Sum(kvp => kvp.Value.Float());

        if (totalApplicableDamage <= ent.Comp.DamageThreshold)
            return;

        var rand = SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent));
        if (rand.Prob(totalApplicableDamage * ent.Comp.ChancePerDamagePoint))
            return;

        if (!_preenable.TrySpawnFeather(ent.Owner, out var feather, ent.Comp.FeatherPrototype))
            return;

        var impulse = FeatherLaunchImpulse + rand.NextFloat(FeatherLaunchImpulseVariance);
        var scatterVec = rand.NextAngle().ToVec() * impulse;
        _physics.ApplyLinearImpulse(feather.Value, scatterVec);

        _popup.PopupEntity(Loc.GetString(ent.Comp.DroppedPopup), ent, ent, PopupType.MediumCaution);
        _chat.TryEmoteWithoutChat(ent, ent.Comp.ScreamEmote);

        _bloodstream.TryAddToBloodstream(ent.Owner, ent.Comp.AdrenalineSolution);
    }
}
