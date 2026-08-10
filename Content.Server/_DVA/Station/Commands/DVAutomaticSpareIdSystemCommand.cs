using System.Diagnostics;
using Content.Server._DVA.Station.Components;
using Content.Server._DVA.Station.Systems;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared._DVA.Station.Commands;

[ToolshedCommand(Name = "spareid"), AdminCommand(AdminFlags.Spawn)]
public sealed partial class DVAutomaticSpareIdSystemCommand : ToolshedCommand
{
    [Dependency] private IEntityManager _entityManager = default!;
    private DVAutomaticSpareIdSystem? _automaticSpareIdSystem;

    [CommandImplementation("unlock")]
    public void Unlock(IInvocationContext ctx, [PipedArgument] EntityUid stationUid, bool doAnnouncement)
    {
        _automaticSpareIdSystem ??= GetSys<DVAutomaticSpareIdSystem>();
        if (!_entityManager.TryGetComponent<DVAutomaticSpareIdComponent>(stationUid, out var spareId))
        {
            ctx.ReportError(new AutomaticSpareIdSystemMissing());
            return;
        }
        _automaticSpareIdSystem.ForceUnlock((stationUid, spareId), null, doAnnouncement ? "command-spareid-unlock-announcement" : null);
    }
}

public record struct AutomaticSpareIdSystemMissing : IConError
{
    public FormattedMessage DescribeInner()
    {
        return FormattedMessage.FromMarkupOrThrow("This command doesn't function if there is no automatic spare ID system. Common usage: stations:get | spareid:unlock true");
    }

    public string? Expression { get; set; }
    public Vector2i? IssueSpan { get; set; }
    public StackTrace? Trace { get; set; }
}
