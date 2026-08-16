namespace Content.Client._DVA.Sprites;

/// <summary>
/// Generic visualizer that sets the colour of layers based on the appearance data.
/// <example>
/// - type: DVGenericColorVisuals
///   visuals:
///     enum.SomeAppearanceData.Key: [layersToApplyItTo]
/// </example>
/// </summary>
[RegisterComponent]
public sealed partial class DVGenericColorVisualsComponent : Component
{
    /// <summary>
    /// Direct sprite visuals.
    /// </summary>
    [DataField(required: true)]
    public Dictionary<Enum, HashSet<string>> Visuals;

    /// <summary>
    /// Clothing visuals.
    /// </summary>
    [DataField]
    public Dictionary<Enum, HashSet<string>>? ClothingVisuals;
}
