using System.Diagnostics.CodeAnalysis;
using Content.Shared.Body;
using Content.Shared.Body.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.Forensics.Systems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Verbs;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Random;

namespace Content.Shared._DVA.Body;

public sealed partial class DVPreenableSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedForensicsSystem _forensics = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedSolutionContainerSystem _solutionContainer = default!;
    [Dependency] private BodySystem _body = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DVPreenableComponent, GetVerbsEvent<Verb>>(OnGetVerbs);
        SubscribeLocalEvent<DVPreenableComponent, PreeningDoAfterEvent>(OnDoAfter);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        var query = EntityQueryEnumerator<DVPreenableComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.RegenerateAt == null || _timing.CurTime <= comp.RegenerateAt)
                continue;

            if (comp.CurrentFeathers >= comp.MaximumFeathers)
            {
                comp.RegenerateAt = null;
                Dirty(uid, comp);
            }

            comp.CurrentFeathers += 1;
            comp.RegenerateAt = comp.CurrentFeathers >= comp.MaximumFeathers
                ? null
                : _timing.CurTime + comp.RegenerationDelay;
            Dirty(uid, comp);
        }
    }

    private void OnGetVerbs(Entity<DVPreenableComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (!args.CanInteract)
            return;

        var user = args.User;

        Verb verb = new()
        {
            Act = () => AttemptDoAfter(ent, user),
            Text = Loc.GetString(ent.Comp.VerbText),
            Disabled = ent.Comp.CurrentFeathers <= 0,
        };
        args.Verbs.Add(verb);
    }

    private void AttemptDoAfter(Entity<DVPreenableComponent> ent, EntityUid user)
    {
        var doArgs = new DoAfterArgs(EntityManager, user, ent.Comp.PreeningDelay, new PreeningDoAfterEvent(), ent, ent)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
        };

        if (user == ent.Owner)
        {
            _popup.PopupEntity(Loc.GetString(ent.Comp.SelfPopup), ent, ent);
        }
        else
        {
            _popup.PopupEntity(Loc.GetString(ent.Comp.RecipientPopup, ("performer", Identity.Entity(user, EntityManager))), ent, ent, PopupType.Medium);
            _popup.PopupEntity(Loc.GetString(ent.Comp.UserPopup, ("recipient", Identity.Entity(ent, EntityManager))), user, user);
        }

        _doAfter.TryStartDoAfter(doArgs);
    }

    private void OnDoAfter(Entity<DVPreenableComponent> ent, ref PreeningDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!TrySpawnFeather(ent.AsNullable(), out var feather))
            return;

        args.Handled = true;
        _hands.TryPickupAnyHand(args.User, feather.Value);
    }

    private Color? GetRandomSkinColor(EntityUid ent)
    {
        if (!_body.TryGetOrgansWithComponent<VisualOrganComponent>(ent, out var visualOrgans))
            return null;

        var rand = SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent));
        return rand.Pick(visualOrgans).Comp.Profile.SkinColor;
    }

    /// <summary>
    /// Attempts to spawn a feather from this entity, taking into account regeneration.
    /// </summary>
    /// <param name="ent">The entity to spawn a feather from.</param>
    /// <param name="feather">The returned feather, if spawned.</param>
    /// <param name="featherOverride">Optional entity to spawn instead of the default one.</param>
    /// <returns>Whether a feather could spawn.</returns>
    [PublicAPI]
    public bool TrySpawnFeather(Entity<DVPreenableComponent?> ent, [NotNullWhen(true)] out EntityUid? feather, EntProtoId? featherOverride = null)
    {
        feather = null;

        if (!Resolve(ent, ref ent.Comp))
            return false;

        if (ent.Comp.CurrentFeathers <= 0)
            return false;

        feather = PredictedSpawnAtPosition(featherOverride ?? ent.Comp.FeatherPrototype, Transform(ent).Coordinates);
        _forensics.TransferDna(feather.Value, ent, false);

        _appearance.SetData(feather.Value, DVFeatherVisuals.FeatherColor, GetRandomSkinColor(ent) ?? Color.White);
        if (TryComp<BloodstreamComponent>(ent, out var bloodstream) &&
            _solutionContainer.ResolveSolution(ent.Owner,
                bloodstream.BloodSolutionName,
                ref bloodstream.BloodSolution,
                out var bloodSolution))
        {
            _appearance.SetData(feather.Value, DVFeatherVisuals.BloodColor, bloodSolution.GetColor(ProtoMan));
        }

        ent.Comp.CurrentFeathers -= 1;
        ent.Comp.RegenerateAt = _timing.CurTime + ent.Comp.RegenerationDelay;
        Dirty(ent);

        return true;
    }
}
