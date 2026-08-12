using Content.Shared._DVA.Nutrition.Components;
using Content.Shared.Item;
using Content.Shared.Nutrition;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Whitelist;

namespace Content.Shared._DVA.Nutrition.EntitySystems;

/// <summary>
/// Handles the time modification of the eaten food
/// </summary>
public sealed class DVEatTimeModifierSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<EdibleEvent>(OnEdible, after: [typeof(IngestionSystem)]);
    }

    private void OnEdible(ref EdibleEvent args)
    {
        var user = args.User;

        if (args.Cancelled)
            return;

        if (!TryComp<DVEatTimeModifierComponent>(user, out var eatTimeModifier))
            return;

        args.Time *= eatTimeModifier.Modifier;
    }
}
