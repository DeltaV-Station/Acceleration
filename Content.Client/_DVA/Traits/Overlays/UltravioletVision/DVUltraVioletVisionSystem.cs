using Content.Shared._DVA.Traits.Components;
using Robust.Client.Graphics;
using Robust.Shared.Player;

namespace Content.Client._DVA.Traits.Overlays.UltravioletVision;

public sealed partial class DVUltravioletVisionSystem : EntitySystem
{
    [Dependency] private IOverlayManager _overlayMan = default!;
    [Dependency] private ISharedPlayerManager _playerMan = default!;

    private DVUltraVisionOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DVUltravioletVisionComponent, ComponentInit>(OnUltraVisionInit);
        SubscribeLocalEvent<DVUltravioletVisionComponent, ComponentShutdown>(OnUltraVisionShutdown);
        SubscribeLocalEvent<DVUltravioletVisionComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<DVUltravioletVisionComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new();
    }

    private void OnUltraVisionInit(EntityUid uid, DVUltravioletVisionComponent component, ComponentInit args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnUltraVisionShutdown(EntityUid uid, DVUltravioletVisionComponent component, ComponentShutdown args)
    {
        if (uid == _playerMan.LocalEntity)
            _overlayMan.RemoveOverlay(_overlay);
    }

    private void OnPlayerAttached(EntityUid uid, DVUltravioletVisionComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnPlayerDetached(EntityUid uid, DVUltravioletVisionComponent component, LocalPlayerDetachedEvent args)
    {
        _overlayMan.RemoveOverlay(_overlay);
    }
}
