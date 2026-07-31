using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedIdCardConsoleSystem))]
public sealed partial class IdCardConsoleComponent : Component
{
    public static string PrivilegedIdCardSlotId = "IdCardConsole-privilegedId";
    public static string TargetIdCardSlotId = "IdCardConsole-targetId";

    [DataField]
    public ItemSlot PrivilegedIdSlot = new();

    [DataField]
    public ItemSlot TargetIdSlot = new();

    [Serializable, NetSerializable]
    public sealed class WriteToTargetIdMessage : BoundUserInterfaceMessage
    {
        public readonly string FullName;
        public readonly string JobTitle;
        public readonly List<ProtoId<AccessLevelPrototype>> AccessList;
        public readonly ProtoId<JobPrototype>? JobPrototype;

        public WriteToTargetIdMessage(string fullName, string jobTitle, List<ProtoId<AccessLevelPrototype>> accessList, ProtoId<JobPrototype>? jobPrototype)
        {
            FullName = fullName;
            JobTitle = jobTitle;
            AccessList = accessList;
            JobPrototype = jobPrototype;
        }
    }

    // Put this on shared so we just send the state once in PVS range rather than every time the UI updates.

    [DataField, AutoNetworkedField]
    public List<ProtoId<AccessLevelPrototype>> AccessLevels = new()
    {
        // DeltaV note: make sure any additions to this list are also added to both:
        //  1. AllAccess in  Resources\Prototypes\Access\misc.yml
        //  2. ComputerIdAdmeme in  Resources\Prototypes\_DV\Entities\Structures\Machines\computers.yml
        "Armory",
        "Atmospherics",
        "Bar",
        //"Brig" Delta V - Removed Brig Access
        "Boxer",  // DeltaV - Add Boxer access
        "Detective",
        "Captain",
        "Cargo",
        "Chapel",
        "Chemistry",
        "ChiefEngineer",
        "ChiefJustice",  // DeltaV - Add Chief Justice access
        "ChiefMedicalOfficer",
        "Clerk", // Delta V - Add Clerk access
        "Clown", // DeltaV - Add Clown access
        "Corpsman", // DeltaV - Add Corpsman access
        "Command",
        "Cryogenics",
        "EmergencyShuttleRepealAll", // DeltaV - fix mismatch with Access/misc.yml
        "Engineering",
        "External",
        "Funding", // DeltaV - Add Funding access
        "GenpopEnter",
        "GenpopLeave",
        "HeadOfPersonnel",
        "HeadOfSecurity",
        "Hydroponics",
        "Janitor",
        "Justice",  // DeltaV - Add Justice access
        "Kitchen",
        "Lawyer",
        "Library",  // DeltaV - Add Library access
        "Mail", // Nyanotrasen - Mail, see Resources/Prototypes/Nyanotrasen/Access/cargo.yml
        "Maintenance",
        "Mantis", // DeltaV - Psionic Mantis, see Resources/Prototypes/_DV/Access/epistemics.yml
        "Medical",
        "Mime", // DeltaV - Add Mime access
        "Musician", // DeltaV - Add Musician access
        "Orders", // DeltaV - Orders, see Resources/Prototypes/_DV/Access/cargo.yml
        "Paramedic", // DeltaV - Add Paramedic access
        "Prosecutor", // Delta V - Add Prosecutor access
        "Psychologist", // DeltaV - Add Psychologist access
        "Quartermaster",
        "Reporter", // DeltaV - Add Reporter access
        "Research",
        "ResearchDirector",
        "Robotics", // DeltaV
        "Salvage",
        "Security",
        "Service",
        "Surgery", // Delta V - Add Surgery access
        "Theatre",
        "Zookeeper",  // DeltaV - Add Zookeeper access
    };

    [Serializable, NetSerializable]
    public sealed class IdCardConsoleBoundUserInterfaceState : BoundUserInterfaceState
    {
        public readonly string PrivilegedIdName;
        public readonly bool IsPrivilegedIdPresent;
        public readonly bool IsPrivilegedIdAuthorized;
        public readonly bool IsTargetIdPresent;
        public readonly string TargetIdName;
        public readonly string? TargetIdFullName;
        public readonly string? TargetIdJobTitle;
        public readonly List<ProtoId<AccessLevelPrototype>>? TargetIdAccessList;
        public readonly List<ProtoId<AccessLevelPrototype>>? AllowedModifyAccessList;
        public readonly ProtoId<JobPrototype> TargetIdJobPrototype;

        public IdCardConsoleBoundUserInterfaceState(bool isPrivilegedIdPresent,
            bool isPrivilegedIdAuthorized,
            bool isTargetIdPresent,
            string? targetIdFullName,
            string? targetIdJobTitle,
            List<ProtoId<AccessLevelPrototype>>? targetIdAccessList,
            List<ProtoId<AccessLevelPrototype>>? allowedModifyAccessList,
            ProtoId<JobPrototype> targetIdJobPrototype,
            string privilegedIdName,
            string targetIdName)
        {
            IsPrivilegedIdPresent = isPrivilegedIdPresent;
            IsPrivilegedIdAuthorized = isPrivilegedIdAuthorized;
            IsTargetIdPresent = isTargetIdPresent;
            TargetIdFullName = targetIdFullName;
            TargetIdJobTitle = targetIdJobTitle;
            TargetIdAccessList = targetIdAccessList;
            AllowedModifyAccessList = allowedModifyAccessList;
            TargetIdJobPrototype = targetIdJobPrototype;
            PrivilegedIdName = privilegedIdName;
            TargetIdName = targetIdName;
        }
    }

    [Serializable, NetSerializable]
    public enum IdCardConsoleUiKey : byte
    {
        Key,
    }
}
