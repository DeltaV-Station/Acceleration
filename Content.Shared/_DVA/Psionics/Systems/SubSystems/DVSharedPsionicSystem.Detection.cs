using System.Linq;
using Content.Shared._DVA.Psionics.Components;
using Content.Shared._DVA.Psionics.Events;
using Content.Shared.Clothing;
using Content.Shared.Popups;

namespace Content.Shared._DVA.Psionics.Systems;

public abstract partial class DVSharedPsionicSystem
{
    [SubscribeLocalEvent]
    private void OnGrantingClothingEquipped(Entity<DVPsionicPowerDetectorComponent> detector, ref ClothingGotEquippedEvent args)
    {
        if (!PotentialQuery.HasComp(args.Wearer)) // IPCs and non-player organics shouldn't be able to use abilities.
            return;

        detector.Comp.Wearer = args.Wearer;
        Dirty(detector);
    }

    [SubscribeLocalEvent]
    private void OnGrantingClothingUnequipped(Entity<DVPsionicPowerDetectorComponent> detector, ref ClothingGotUnequippedEvent args)
    {
        detector.Comp.Wearer = null;
        Dirty(detector);
    }

    [SubscribeLocalEvent]
    private void OnPowerUsed(Entity<DVPsionicComponent> psionic, ref DVPsionicPowerUsedEvent args)
    {
        var coords = Transform(args.User).Coordinates;

        foreach (var detector in _lookup.GetEntitiesInRange<DVPsionicPowerDetectorComponent>(coords, 10f)
                     .Select(detectorPower => detectorPower.Comp.Wearer ?? detectorPower.Owner))
        {
            if (detector == args.User)
                continue;

            if (!CanUsePsionicPower(detector))
                continue;
            // This is for artifacts.
            var detectEv = new DVPsionicPowerDetectedEvent(args.User, args.Power);
            RaiseLocalEvent(detector, ref detectEv);

            Popup.PopupEntity(Loc.GetString("psionic-power-metapsionic-power-detected", ("power", args.Power)), detector, detector, PopupType.LargeCaution);
        }

        args.Handled = true;
    }
}
