using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared.Eye;

namespace Content.Shared._DVA.Psionics.Systems;

public abstract partial class DVSharedPsionicSystem
{
    [Dependency] private SharedEyeSystem _eye = default!;
    [Dependency] private SharedVisibilitySystem _visibility = default!;

    [SubscribeLocalEvent]
    private void OnInvisInit(Entity<DVPsionicallyInvisibleComponent> invisible, ref MapInitEvent args)
    {
        if (!CanUsePsionicPower(invisible))
            invisible.Comp.Active = false;

        SetPsionicInvisibility(invisible.Owner, invisible.Comp.Active);
    }

    [SubscribeLocalEvent]
    private void OnInvisShutdown(Entity<DVPsionicallyInvisibleComponent> invisible, ref ComponentShutdown args)
    {
        SetPsionicInvisibility(invisible.Owner, false);
    }

    [SubscribeLocalEvent]
    private void OnInit(Entity<DVPotentialPsionicComponent> potPsionic, ref MapInitEvent args)
    {
        SetCanSeePsionicInvisiblity(potPsionic, false);
    }

    [SubscribeLocalEvent]
    private void OnSuppression(Entity<DVPsionicallyInvisibleComponent> invisible, ref DVPsionicSuppressedEvent args)
    {
        invisible.Comp.Active = false;
        SetPsionicInvisibility(invisible.Owner, invisible.Comp.Active);
    }

    [SubscribeLocalEvent]
    private void OnSuppressionStop(Entity<DVPsionicallyInvisibleComponent> invisible, ref DVPsionicSuppressionStoppedEvent args)
    {
        // This event only raises when they can use psionic abilities again, so no need for a check.
        invisible.Comp.Active = true;
        SetPsionicInvisibility(invisible.Owner, invisible.Comp.Active);
    }

    [SubscribeLocalEvent]
    private void OnShielded(Entity<DVPotentialPsionicComponent> potPsionic, ref DVPsionicShieldedEvent args)
    {
        SetCanSeePsionicInvisiblity(potPsionic, true);
    }

    [SubscribeLocalEvent]
    private void OnShieldedStop(Entity<DVPotentialPsionicComponent> potPsionic, ref DVPsionicShieldingStoppedEvent args)
    {
        //This only gets raised when they are no longer shielded, so no need to check if they're still shielded by something else.
        var ev = new DVCheckPsionicSightEvent(potPsionic);
        RaiseLocalEvent(potPsionic, ref ev);

        if (!ev.CanSeePsionicInvisible)
            SetCanSeePsionicInvisiblity(potPsionic, false);
    }

    /// <summary>
    /// Enables or disables an entity to see a psionically invisible entity.
    /// </summary>
    /// <param name="potPsionic">The entity whose ability to see psionically invisible entities is being changed.</param>
    /// <param name="canSee">Whether they can see psionically invisible entities.</param>
    public void SetCanSeePsionicInvisiblity(EntityUid potPsionic, bool canSee)
    {
        if (!TryComp<EyeComponent>(potPsionic, out var eye))
            return;

        if (canSee)
            _eye.SetVisibilityMask(potPsionic, eye.VisibilityMask | (int) VisibilityFlags.PsionicallyInvisible, eye);
        else
            _eye.SetVisibilityMask(potPsionic, eye.VisibilityMask & ~ (int) VisibilityFlags.PsionicallyInvisible, eye);
    }

    /// <summary>
    /// Set their psionic invisibility.
    /// </summary>
    /// <param name="visible">The entity that attempts to toggle their psionic invisibility.</param>
    /// <param name="invisible">Whether they're visible or invisible.</param>
    public void SetPsionicInvisibility(Entity<VisibilityComponent?> visible, bool invisible)
    {
        if (invisible)
        {
            // Remove them from the normal layer and add them to the psionic layer.
            _visibility.AddLayer(visible, (int) VisibilityFlags.PsionicallyInvisible, false);
            _visibility.RemoveLayer(visible, (int) VisibilityFlags.Normal);
        }
        else
        {
            // Remove them from the psionic layer and add them to the normal layer.
            _visibility.RemoveLayer(visible, (int) VisibilityFlags.PsionicallyInvisible, false);
            _visibility.AddLayer(visible, (int) VisibilityFlags.Normal);
        }
    }
}
