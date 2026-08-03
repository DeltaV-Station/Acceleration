using System.Linq;
using JetBrains.Annotations;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._DVA.Glimmer;

/// <summary>
/// Oversees the glimmer entity associated with a round.
/// </summary>
public sealed partial class DVGlimmerSystem : EntitySystem
{
    [Dependency] private INetManager _net = default!;

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
    /// The maximum glimmer possible.
    /// </summary>
    [PublicAPI]
    public int MaxGlimmer
    {
        get => Entity?.Comp.MaxGlimmer ?? 0;
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

        SubscribeLocalEvent<DVGlimmerComponent, ComponentGetState>(OnGetState);
        SubscribeLocalEvent<DVGlimmerComponent, ComponentHandleState>(OnHandleState);
    }

    [Serializable, NetSerializable]
    public sealed class DVGlimmerState : IComponentState
    {
        public int Glimmer;
        public int MinGlimmer;
        public int MaxGlimmer;
        public Dictionary<int, GlimmerTier> Tiers = default!;
    }

    private void OnGetState(Entity<DVGlimmerComponent> ent, ref ComponentGetState args)
    {
        args.State = new DVGlimmerState
        {
            Glimmer = ent.Comp.Glimmer,
            MinGlimmer = ent.Comp.MinGlimmer,
            MaxGlimmer = ent.Comp.MaxGlimmer,
            Tiers = ent.Comp.Tiers.ToDictionary(),
        };
    }

    private void OnHandleState(Entity<DVGlimmerComponent> ent, ref ComponentHandleState args)
    {
        if (args.Current is not DVGlimmerState state)
            return;

        ent.Comp.Glimmer = state.Glimmer;
        ent.Comp.MinGlimmer = state.MinGlimmer;
        ent.Comp.MaxGlimmer = state.MaxGlimmer;
        ent.Comp.Tiers = new SortedDictionary<int, GlimmerTier>(state.Tiers);

        var passive = new GlimmerChangedPassiveEvent(ent);
        RaiseLocalEvent(ref passive);
    }
}
