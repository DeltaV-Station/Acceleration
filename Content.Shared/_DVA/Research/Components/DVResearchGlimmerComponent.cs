namespace Content.Shared._DVA.Research.Components;

/// <summary>
/// Component for handling multipliers and Glimmer production when players generate
/// research via an analysis console.
/// </summary>
[RegisterComponent]
public sealed partial class DVAnalysisConsoleGlimmerComponent : Component
{
    /// <summary>
    /// How much research is required to generate a single point of Glimmer.
    /// </summary>
    [DataField]
    public float ResearchPerGlimmer = 1250;
}
