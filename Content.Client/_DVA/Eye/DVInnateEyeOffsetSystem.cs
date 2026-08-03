using System.Numerics;
using Content.Client.Movement.Components;
using Content.Client.Movement.Systems;
using Content.Shared._DVA.Eye;

namespace Content.Client._DVA.Eye;

public sealed partial class DVInnateEyeOffsetSystem : DVSharedInnateEyeOffsetSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DVInnateEyeOffsetComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<DVInnateEyeOffsetComponent, AfterAutoHandleStateEvent>(OnHandleState);
    }

    private void OnInit(Entity<DVInnateEyeOffsetComponent> ent, ref ComponentInit args)
    {
        UpdateEyeOffset(ent);
    }

    private void UpdateEyeOffset(Entity<DVInnateEyeOffsetComponent> ent)
    {
        if (!TryComp<EyeCursorOffsetComponent>(ent, out var cursorOffsetComp))
            return;

        if (!ent.Comp.Active)
        {
            cursorOffsetComp.CurrentPosition = Vector2.Zero;
            cursorOffsetComp.TargetPosition = Vector2.Zero;
        }
        cursorOffsetComp.Active = ent.Comp.Active;
    }

    private void OnHandleState(Entity<DVInnateEyeOffsetComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        UpdateEyeOffset(ent);
    }

    public override void SetActive(Entity<DVInnateEyeOffsetComponent> ent, bool active)
    {
        base.SetActive(ent, active);

        UpdateEyeOffset(ent);
    }
}
