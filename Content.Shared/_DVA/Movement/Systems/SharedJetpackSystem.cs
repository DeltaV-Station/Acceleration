using Content.Shared._DVA.Movement.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;

namespace Content.Shared.Movement.Systems;

public abstract partial class SharedJetpackSystem
{
    [Dependency] private SharedTransformSystem _transform = default!;

    private void OnJetpackToggle(Entity<JetpackComponent> jetpack, ref ToggleJetpackEvent args)
    {
        if (args.Handled)
            return;

        jetpack.Comp.AutomaticMode = !jetpack.Comp.AutomaticMode;
        jetpack.Comp.AutomaticUser = args.Performer;
        Dirty(jetpack);

        if (!CanEnableOnGrid(_transform.GetGrid(jetpack.Owner)))
        {
            if (jetpack.Comp.AutomaticMode)
            {
                var autoUser = EnsureComp<DVAutomaticJetpackUserComponent>(args.Performer);
                autoUser.Jetpack = jetpack;
                Dirty(args.Performer, autoUser);
            }
            else
            {
                RemComp<DVAutomaticJetpackUserComponent>(args.Performer);
            }

            var message = jetpack.Comp.AutomaticMode ? "jetpack-activated-on-grid" : "jetpack-deactivated";
            _popup.PopupEntity(Loc.GetString(message), jetpack, args.Performer);
            return;
        }

        var messageOffGrid = jetpack.Comp.AutomaticMode ? "jetpack-activated-off-grid" : "jetpack-deactivated";
        _popup.PopupEntity(Loc.GetString(messageOffGrid), jetpack, args.Performer);

        SetEnabled(jetpack.Owner, jetpack.Comp, !IsEnabled(jetpack.Owner));
    }

    private void OnAutomaticJetpackEntParentChanged(Entity<DVAutomaticJetpackUserComponent> jetpackUser, ref EntParentChangedMessage args)
    {
        if (!TryComp<JetpackComponent>(jetpackUser.Comp.Jetpack, out var jetpack)
            || args.Transform.GridUid != null)
            return;

        SetEnabled(jetpackUser.Comp.Jetpack, jetpack, true, args.Entity);
        _popup.PopupEntity(Loc.GetString("jetpack-activates-automatically"), args.Entity, args.Entity);
    }

    private void RemoveAutomaticJetpack(Entity<JetpackComponent> jetpack)
    {
        jetpack.Comp.AutomaticMode = false;
        if (jetpack.Comp.AutomaticUser.HasValue)
            RemComp<DVAutomaticJetpackUserComponent>(jetpack.Comp.AutomaticUser.Value);
        jetpack.Comp.AutomaticUser = null;
    }

    private void RefreshAutomaticJetpack(Entity<JetpackComponent> jetpack, EntityUid user, bool jetpackEnabled)
    {
        if (jetpackEnabled)
            RemComp<DVAutomaticJetpackUserComponent>(user);
        else if (jetpack.Comp.AutomaticMode) // DeltaV - Jetpacks automatically turn on when toggled.
            EnsureComp<DVAutomaticJetpackUserComponent>(user).Jetpack = jetpack;

    }
}
