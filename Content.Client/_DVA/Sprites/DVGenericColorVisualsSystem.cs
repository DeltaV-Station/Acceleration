using Content.Client.Clothing;
using Content.Shared.Clothing.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Reflection;

namespace Content.Client._DVA.Sprites;

public sealed partial class DVGenericColorVisualsSystem : VisualizerSystem<DVGenericColorVisualsComponent>
{
    [Dependency] private IReflectionManager _refMan = default!;
    [Dependency] private SharedAppearanceSystem _appearanceSys = default!;
    [Dependency] private ClientClothingSystem _clothing = default!;

    protected override void OnAppearanceChange(EntityUid uid, DVGenericColorVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        foreach (var (appearanceKey, layers) in component.Visuals)
        {
            if (!_appearanceSys.TryGetData<Color>(uid, appearanceKey, out var color, args.Component))
                continue;

            foreach (var layer in layers)
            {
                var layerIndex = _refMan.TryParseEnumReference(layer, out var @enum)
                    ? SpriteSystem.LayerMapGet((uid, args.Sprite), @enum)
                    : SpriteSystem.LayerMapGet((uid, args.Sprite), layer);

                SpriteSystem.LayerSetColor((uid, args.Sprite), layerIndex, color);
            }
        }

        if (component.ClothingVisuals is null || !TryComp<ClothingComponent>(uid, out var clothing))
            return;

        foreach (var (appearanceKey, layers) in component.ClothingVisuals)
        {
            if (!_appearanceSys.TryGetData<Color>(uid, appearanceKey, out var color, args.Component))
                continue;

            foreach (var layer in layers)
            {
                foreach (var slotPair in clothing.ClothingVisuals)
                {
                    _clothing.SetLayerColor(clothing, slotPair.Key, layer, color);
                }
            }
        }
    }
}
