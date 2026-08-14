using Content.Shared._DVA.Glimmer;
using Content.Shared._DVA.Psionics.Components.Powers;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._DVA.Psionics.Systems.PsionicPowers;

/// <summary>
/// This is a base psionic power system that handles being mindbroken and checks for being able to use psionic powers automagically!
/// You WILL NEED to parent of this.
/// </summary>
public abstract partial class DVBasePsionicPowerSystem<TPower, TEvent> : EntitySystem where TPower : Component where TEvent : BaseActionEvent
{
    [Dependency] private ISharedAdminLogManager _adminLogger = default!;
    [Dependency] protected IRobustRandom Random = default!;
    [Dependency] protected IGameTiming Timing = default!;
    [Dependency] protected SharedActionsSystem Action = default!;
    [Dependency] protected SharedDoAfterSystem DoAfter = default!;
    [Dependency] private DVGlimmerSystem _glimmer = default!;
    [Dependency] protected SharedPopupSystem Popup = default!;
    [Dependency] protected DVSharedPsionicSystem Psionic = default!;

    [Dependency] private EntityQuery<DVPsionicPowerComponent> _powerQuery = default!;

    // We can't use the [SubscribeLocalEvent] attribute because we're using generic types.
    // Probably a skissue on my part, but it didn't work without.
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TPower, TEvent>(OnPowerActionUsed);
    }

    /// <summary>
    /// This is called whenever an entity pushes the psionic power action button.
    /// </summary>
    /// <param name="psionic">The psionic who attempts to use a psionic power.</param>
    /// <param name="args">The action event for said power.</param>
    private void OnPowerActionUsed(Entity<TPower> psionic, ref TEvent args)
    {
        if (Timing.ApplyingState || args.Handled)
            return;

        if (CanUsePsionicPower(psionic, args.Performer))
        {
            OnPowerUsed(psionic, ref args);
            AfterPowerUsed(psionic, args.Performer);
            args.Handled = true;
            return;
        }

        Popup.PopupEntity(Loc.GetString("psionic-cannot-use-psionics"), args.Performer, args.Performer);
    }

    /// <summary>
    /// This is the creme of the system. If you add a new power, you will want to call this.
    /// This is where the actual power takes place that follows after the button press.
    /// You will need to call Psionic.CanBeTargeted(target) if you add a power that can target things.
    /// IMPORTANT! The psionic entity DOES NOT HAVE TO BE THE PLAYER! It can be a clothing!
    /// If you want something to affect the player everytime, use args.Performer!
    /// psionic.Owner => Source of the power. args.Performer => The user of the power.
    /// </summary>
    /// <param name="psionic">The source of the power.</param>
    /// <param name="args">The event.</param>
    protected abstract void OnPowerUsed(Entity<TPower> psionic, ref TEvent args);

    #region Helpermethods
    /// <summary>
    /// This will log the power usage and increase glimmer, as well as making sure metapsionics hear it.
    /// </summary>
    /// <param name="psionicSource">The SOURCE of the psionic power.</param>
    /// <param name="performer">The entity that PERFORMED the power.</param>
    private void AfterPowerUsed(EntityUid psionicSource, EntityUid performer)
    {
        if (!_powerQuery.TryComp(psionicSource, out var powerComp))
        {
            DebugTools.Assert($"The psionic power '{psionicSource}' didn't have the 'DVPsionicPowerComponent' component.");
        }

        _adminLogger.Add(Database.LogType.Psionics, Database.LogImpact.Medium, $"{ToPrettyString(performer):player} used {powerComp.PowerName}.");

        var ev = new DVPsionicPowerUsedEvent(performer, psionicSource, powerComp.PowerName);
        RaiseLocalEvent(psionicSource, ev);

        _glimmer.Glimmer += Random.Next(powerComp.MinGlimmerChanged, powerComp.MaxGlimmerChanged);
    }

    /// <summary>
    /// This checks whether a psionic can use their power.
    /// If this returns false, the action will not go on cooldown.
    /// This can be overriden for power specific checks, such as mindswaps requiring no mindshields.
    /// </summary>
    /// <param name="psionic">The source of the psionic power.</param>
    /// <param name="performer">The performer who attempts to use the power.</param>
    /// <returns>Returns true if they're able to use powers, otherwise false.</returns>
    protected virtual bool CanUsePsionicPower(Entity<TPower> psionic, EntityUid performer)
    {
        return Psionic.CanUsePsionicPower(performer);
    }
    #endregion
}
