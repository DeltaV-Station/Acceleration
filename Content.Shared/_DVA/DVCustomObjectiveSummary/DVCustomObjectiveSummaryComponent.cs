using Robust.Shared.GameStates;

namespace Content.Shared._DVA.DVCustomObjectiveSummary;

/// <summary>
///     Put on a players mind if the wrote a custom summary for their objectives.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DVCustomObjectiveSummaryComponent : Component
{
    /// <summary>
    ///     What the player wrote as their summary!
    /// </summary>
    [DataField, AutoNetworkedField]
    public string ObjectiveSummary = "";
}
