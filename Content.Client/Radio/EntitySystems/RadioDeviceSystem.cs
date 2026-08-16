using Content.Client.Radio.Ui;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Robust.Client.GameObjects;

namespace Content.Client.Radio.EntitySystems;

public sealed partial class RadioDeviceSystem : SharedRadioDeviceSystem
{
    [Dependency] private UserInterfaceSystem _ui = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        // BEGIN DeltaV - Update intercom UI on RadioMicrophone/Speaker component statechange
        base.Initialize();
        SubscribeLocalEvent<IntercomComponent, AfterAutoHandleStateEvent>(OnAfterHandleState);
        SubscribeLocalEvent<RadioMicrophoneComponent, AfterAutoHandleStateEvent>(OnAfterHandleState);
        SubscribeLocalEvent<RadioSpeakerComponent, AfterAutoHandleStateEvent>(OnAfterHandleState);
        // END DeltaV
    }

    /// <summary>
    /// DeltaV - Update intercom UI on RadioMicrophone/Speaker component statechange.
    /// </summary>
    private void OnAfterHandleState<TComp>(EntityUid uid, TComp component, AfterAutoHandleStateEvent args) where TComp : IComponent
    {
        IntercomComponent? intercom = null;
        if (!Resolve(uid, ref intercom, false))
            return;

        var entity = new Entity<IntercomComponent>(uid, intercom);

        if (_ui.TryGetOpenUi<IntercomBoundUserInterface>(entity.Owner, IntercomUiKey.Key, out var bui))
            bui.Update(entity);
    }

    /* DeltaV - Replaced by OnAfterHandleState
    private void OpenAndUpdateUI(Entity<IntercomComponent> ent)
    {
        if (_ui.TryGetOpenUi<IntercomBoundUserInterface>(ent.Owner, IntercomUiKey.Key, out var bui))
            bui.Update(ent);
    }
    */
}
