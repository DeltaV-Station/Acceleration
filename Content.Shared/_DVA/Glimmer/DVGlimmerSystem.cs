using JetBrains.Annotations;

namespace Content.Shared._DVA.Glimmer;

/// <summary>
/// Oversees the glimmer entity associated with a round.
/// </summary>
public sealed class DVGlimmerSystem : EntitySystem
{
    /// <summary>
    /// The current glimmer of the round.
    /// </summary>
    [PublicAPI]
    public int Glimmer
    {
        get => Entity?.Comp.Glimmer ?? 0;
        set
        {
            if (Entity is not { } ent)
                return;

            SetGlimmer(ent, value);
        }
    }

    /// <summary>
    /// The current glimmer tier of the round.
    /// </summary>
    [PublicAPI]
    public GlimmerTier GlimmerTier
    {
        get => Entity?.Comp.Tier ?? GlimmerTier.Minimal;
    }

    /// <summary>
    /// Returns the current glimmer entity for the round, if any.
    /// </summary>
    [PublicAPI]
    public Entity<DVGlimmerComponent>? Entity
    {
        get
        {
            var query = EntityQueryEnumerator<DVGlimmerComponent>();
            while (query.MoveNext(out var uid, out var comp))
            {
                return (uid, comp);
            }

            return null;
        }
    }

    /// <summary>
    /// Sets the glimmer of the given glimmer-entity.
    /// </summary>
    /// <remarks>
    /// There should likely only be one of these, see <see cref="Glimmer" /> and <see cref="GlimmerTier" />
    /// </remarks>
    [PublicAPI]
    public void SetGlimmer(Entity<DVGlimmerComponent> ent, int glimmer)
    {
        if (ent.Comp.Glimmer == glimmer)
            return;

        ent.Comp.Glimmer = glimmer;
        Dirty(ent);

        var active = new GlimmerChangedActiveEvent(ent);
        RaiseLocalEvent(ref active);

        var passive = new GlimmerChangedPassiveEvent(ent);
        RaiseLocalEvent(ref passive);
    }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DVGlimmerComponent, AfterAutoHandleStateEvent>(OnAfterHandleState);
    }

    private void OnAfterHandleState(Entity<DVGlimmerComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        var passive = new GlimmerChangedPassiveEvent(ent);
        RaiseLocalEvent(ref passive);
    }
}
