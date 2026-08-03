using Content.Shared.Camera;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using JetBrains.Annotations;

namespace Content.Shared._DVA.Eye;

public abstract partial class DVSharedInnateEyeOffsetSystem : EntitySystem
{
    [Dependency] private SharedContentEyeSystem _contentEye = default!;
    [Dependency] private IComponentFactory _componentFactory = default!;

    private Type _eyeType = default!;

    public override void Initialize()
    {
        base.Initialize();

        _eyeType = _componentFactory.GetRegistration("EyeCursorOffset").Type;

        SubscribeLocalEvent<DVInnateEyeOffsetComponent, GetEyePvsScaleEvent>(OnGetEyePvsScale);
        SubscribeLocalEvent<DVInnateEyeOffsetComponent, DVToggleInnateEyeOffsetEvent>(OnToggleInnateEyeOffset);
    }

    [PublicAPI]
    public virtual void SetActive(Entity<DVInnateEyeOffsetComponent> ent, bool active)
    {
        if (ent.Comp.Active == active)
            return;

        ent.Comp.Active = active;
        Dirty(ent);

        _contentEye.UpdatePvsScale(ent);
    }

    private void OnGetEyePvsScale(Entity<DVInnateEyeOffsetComponent> ent, ref GetEyePvsScaleEvent args)
    {
        if (!TryComp(ent, _eyeType, out var eyeCursorOffset))
            return;

        var factor = ((SharedEyeCursorOffsetComponent)eyeCursorOffset).PvsIncrease;

        if (!ent.Comp.Active)
            return;

        args.Scale += factor;
    }

    private void OnToggleInnateEyeOffset(Entity<DVInnateEyeOffsetComponent> ent, ref DVToggleInnateEyeOffsetEvent args)
    {
        SetActive(ent, !ent.Comp.Active);
    }
}
