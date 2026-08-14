using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._DVA.Psionics.Components;

/// <summary>
/// An entity with this component will be deleted when dispelled.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DVDeleteOnDispellComponent : Component
{
    /// <summary>
    /// The sound that occurs when being dispelled.
    /// </summary>
    [DataField]
    public SoundSpecifier DispelSound = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg");
}
