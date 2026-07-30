using Content.Shared.Administration;
using Content.Shared.Commands;
using Robust.Shared.Console;
using Content.Server._DVA.Station.Systems;

namespace Content.Server.Administration.Commands
{
    [AdminCommand(AdminFlags.Spawn)]
    public sealed partial class UnlockSpareIdCommand : LocalizedEntityCommands
    {
        [Dependency] private DVAutomaticSpareIdSystem _automaticSpareIdSystem = default!;

        public override string Command => "unlockspareid";
        public override string Description => Loc.GetString("cmd-unlockspareid-desc");

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (!CommandHelper.CheckExactlyOneArgument(Loc, shell, args))
                return;

            if (!CommandHelper.ParseArgumentBoolean(Loc, shell, args[0], out var boolean))
                return;

            _automaticSpareIdSystem.ForceUnlock(null, boolean ? "cmd-unlockspareid-announcement" : null);
        }

        public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length == 1)
            {
                return CompletionResult.FromHintOptions(
                    CompletionHelper.Booleans,
                    Loc.GetString("cmd-unlockspareid-arg-state"));
            }

            return CompletionResult.Empty;
        }
    }
}