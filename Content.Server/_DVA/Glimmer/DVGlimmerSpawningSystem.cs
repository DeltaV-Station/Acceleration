using Content.Server.GameTicking;
using Content.Shared._DVA.Glimmer;
using Robust.Server.GameStates;

namespace Content.Server._DVA.Glimmer;

/// <summary>
/// Responsible for spawning the glimmer entity within a round.
/// </summary>
public sealed partial class DVGlimmerSpawningSystem : EntitySystem
{
    [Dependency] private PvsOverrideSystem _pvsOverride = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PostGameMapLoad>(OnPostGameMapLoad);
    }

    private void OnPostGameMapLoad(PostGameMapLoad ev)
    {
        var ent = Spawn();
        _ = AddComp<DVGlimmerComponent>(ent);
        _pvsOverride.AddGlobalOverride(ent);
    }
}
