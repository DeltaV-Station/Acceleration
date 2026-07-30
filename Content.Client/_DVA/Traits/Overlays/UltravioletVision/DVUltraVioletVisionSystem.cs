using Content.Shared._DVA.Traits.Components;
using Robust.Client.Graphics;
using Robust.Shared.Player;

namespace Content.Client._DVA.Traits.Overlays.UltraVioletVision;

public sealed partial class DVUltraVioletVisionSystem : EntitySystem
{
    [Dependency] private IOverlayManager _overlayMan = default!;
    [Dependency] private ISharedPlayerManager _playerMan = default!;

    private DVUltraVisionOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DVUltraVioletVisionComponent, ComponentInit>(OnUltraVisionInit);
        SubscribeLocalEvent<DVUltraVioletVisionComponent, ComponentShutdown>(OnUltraVisionShutdown);
        SubscribeLocalEvent<DVUltraVioletVisionComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<DVUltraVioletVisionComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new();
    }

    private void OnUltraVisionInit(EntityUid uid, DVUltraVioletVisionComponent component, ComponentInit args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnUltraVisionShutdown(EntityUid uid, DVUltraVioletVisionComponent component, ComponentShutdown args)
    {
        if (uid == _playerMan.LocalEntity)
            _overlayMan.RemoveOverlay(_overlay);
    }

    private void OnPlayerAttached(EntityUid uid, DVUltraVioletVisionComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnPlayerDetached(EntityUid uid, DVUltraVioletVisionComponent component, LocalPlayerDetachedEvent args)
    {
        _overlayMan.RemoveOverlay(_overlay);
    }
}
