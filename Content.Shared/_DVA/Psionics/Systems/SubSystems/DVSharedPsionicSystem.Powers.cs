using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Components.Powers;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Popups;
using Robust.Shared.Containers;

namespace Content.Shared._DVA.Psionics.Systems;

public abstract partial class DVSharedPsionicSystem
{
    [Dependency] private SharedActionsSystem _action = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;

    [Dependency] private EntityQuery<DVCannotUsePsionicsComponent> _noPsionicsQuery = default!;

    [SubscribeLocalEvent]
    private void OnPowerMapInit(Entity<DVPsionicPowerComponent> power, ref MapInitEvent args)
    {
        _action.AddAction(power, ref power.Comp.ActionEntity, power.Comp.ActionProtoId);
        Dirty(power);
    }

    [SubscribeLocalEvent]
    private void OnGetPowerEntitiesEvent(Entity<DVPsionicPowerComponent> power, ref DVPsionicPowerRelayedEvent<DVGetPsionicPowerEntitiesEvent> args)
    {
        args.Args.PsionicPowerEntities.Add(power);
    }

    [SubscribeLocalEvent]
    private void OnDispelled(Entity<DVPsionicPowerComponent> psionic, ref DVPsionicPowerRelayedEvent<MindRelayedEvent<DVDispelledEvent>> args)
    {
        if (args.Args.Args.Handled)
            return;

        args.Args.Args.Handled = TryStopPsionicDoAfter(psionic, args.Args.Args.Dispeller, Loc.GetString("psionic-dispelled"));
    }

    [SubscribeLocalEvent]
    private void OnMindBroken(Entity<DVPsionicPowerComponent> psionic, ref DVPsionicPowerRelayedEvent<MindRelayedEvent<DVPsionicMindBrokenEvent>> args)
    {
        if (psionic.Comp.CanBeRemoved || args.Args.Args.Force)
        {
            args.Args.Args.Success = true;
            TryStopPsionicDoAfter(psionic, psionic.Owner); // Mindbreaking already has a popup, so no popup for breaking doAfters.
            PredictedQueueDel(psionic);
            return;
        }

        args.Args.Args.AllRemoved = false;
    }

    [SubscribeLocalEvent]
    private void OnPsionicallySuppressed(Entity<DVPsionicPowerComponent> power, ref DVPsionicSuppressedEvent args)
    {
        if (_timing.ApplyingState)
            return;

        TryStopPsionicDoAfter(power, args.Victim, Loc.GetString("psionic-equipped-shielded-in-doafter"));
    }

    private bool TryStopPsionicDoAfter(Entity<DVPsionicPowerComponent> psionic, EntityUid performer, [ForbidLiteral] string? message = null)
    {
        if (psionic.Comp.GetDoAfterId() is not { } doAfterId)
            return false;

        _doAfter.Cancel(doAfterId);
        Popup.PopupEntity(message, performer, performer, PopupType.MediumCaution);

        psionic.Comp.RemoveSavedDoAfterId();
        Dirty(psionic);
        return true;
    }

    #region PsionicContainer
    [SubscribeLocalEvent]
    private void OnPsionicPowerContainerInit(Entity<DVPsionicPowersContainerComponent> psionic, ref ComponentInit args)
    {
        psionic.Comp.PsionicPowersContainer = _container.EnsureContainer<Container>(psionic, DVPsionicPowersContainerComponent.ContainerId);
        // We show the contents of the container to allow psionic powers to have visible sprites.
        psionic.Comp.PsionicPowersContainer.ShowContents = true;
        psionic.Comp.PsionicPowersContainer.OccludesLight = false;
    }

    [SubscribeLocalEvent]
    private void OnStatusContainerShutdown(Entity<DVPsionicPowersContainerComponent> psionic, ref ComponentShutdown args)
    {
        if (psionic.Comp.PsionicPowersContainer is { } container)
            _container.ShutdownContainer(container);
    }

    [SubscribeLocalEvent]
    private void OnEntityInserted(Entity<DVPsionicPowersContainerComponent> psionic, ref EntInsertedIntoContainerMessage args)
    {
        if (args.Container.ID != DVPsionicPowersContainerComponent.ContainerId
            || psionic.Comp.AttachedEntity is not { } attachedEntity)
            return;

        _action.GrantContainedActions(attachedEntity, args.Entity);
    }

    [SubscribeLocalEvent]
    private void OnEntityInserted(Entity<DVPsionicPowersContainerComponent> psionic, ref EntRemovedFromContainerMessage args)
    {
        if (args.Container.ID != DVPsionicPowersContainerComponent.ContainerId
            || psionic.Comp.AttachedEntity is not { } attachedEntity)
            return;

        _action.RemoveProvidedActions(attachedEntity, args.Entity);
    }

    [SubscribeLocalEvent]
    private void OnMindRemoval(Entity<DVPsionicPowersContainerComponent> psionic, ref MindGotRemovedEvent args)
    {
        if (psionic.Comp.AttachedEntity is not { } attachedEntity || TerminatingOrDeleted(attachedEntity))
            return;

        var ev = new DVGetPsionicPowerEntitiesEvent();
        RaiseLocalEvent(args.Mind, ref ev);

        foreach (var powerEntity in ev.PsionicPowerEntities)
        {
            _action.RemoveProvidedActions(attachedEntity, powerEntity);
        }

        psionic.Comp.AttachedEntity = null;
        Dirty(psionic);
    }

    [SubscribeLocalEvent]
    private void OnMindAddition(Entity<DVPsionicPowersContainerComponent> psionic, ref MindGotAddedEvent args)
    {
        if (_noPsionicsQuery.HasComp(args.Container))
            return;

        var ev = new DVGetPsionicPowerEntitiesEvent();
        RaiseLocalEvent(args.Mind, ref ev);

        foreach (var powerEntity in ev.PsionicPowerEntities)
        {
            _action.GrantContainedActions(args.Container.Owner, powerEntity);
        }

        psionic.Comp.AttachedEntity = args.Container;
        Dirty(psionic);
    }
    #endregion
}
