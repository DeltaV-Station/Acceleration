using Content.Shared._DVA.Traits;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;


namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor : BoxContainer
{
    /// Called when trait selection changes in the TraitsTab.
    /// Updates the profile with the new trait selection.
    /// </summary>
    private void OnTraitsSelectionChanged(HashSet<ProtoId<DVTraitPrototype>> traits)
    {
        if (Profile is null)
            return;

        // Remove all existing traits - iterate directly over readonly collection
        foreach (var existingTrait in Profile.TraitPreferences)
        {
            Profile = Profile.WithoutTraitPreference(existingTrait, _prototypeManager);
        }

        // Add newly selected traits
        foreach (var trait in traits)
        {
            Profile = Profile.WithTraitPreference(trait.Id, _prototypeManager);
        }

        SetDirty();
    }

    /// <summary>
    /// Updates the traits tab with the current profile's selected traits.
    /// </summary>
    private void UpdateTraitsSelection()
    {
        if (Profile is null)
        {
            Traits.SetSelectedTraits(new HashSet<ProtoId<DVTraitPrototype>>());
            return;
        }

        // Convert profile's trait preferences (strings) to ProtoId<TraitPrototype>
        var selectedTraits = new HashSet<ProtoId<DVTraitPrototype>>(Profile.TraitPreferences.Count);
        foreach (var traitId in Profile.TraitPreferences)
        {
            // Validate that the trait still exists in prototypes
            if (_prototypeManager.HasIndex(traitId))
            {
                selectedTraits.Add(new ProtoId<DVTraitPrototype>(traitId));
            }
        }

        Traits.SetSelectedTraits(selectedTraits);
        Traits.UpdateConditions(Profile.Species);
    }
}