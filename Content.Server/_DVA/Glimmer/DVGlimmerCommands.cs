using System.Diagnostics;
using Content.Server.Administration;
using Content.Shared._DVA.Glimmer;
using Content.Shared.Administration;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Content.Server._DVA.Glimmer;

[ToolshedCommand(Name = "glimmer"), AdminCommand(AdminFlags.Query | AdminFlags.Fun)]
public sealed class DVGlimmerCommands : ToolshedCommand
{
    private DVGlimmerSystem? _glimmer;

    [CommandImplementation("get")]
    public int Get()
    {
        _glimmer ??= GetSys<DVGlimmerSystem>();
        return _glimmer.Glimmer;
    }

    [CommandImplementation("tier")]
    public GlimmerTier Tier()
    {
        _glimmer ??= GetSys<DVGlimmerSystem>();
        return _glimmer.GlimmerTier;
    }

    [CommandImplementation("set")]
    public void Set(int glimmer)
    {
        _glimmer ??= GetSys<DVGlimmerSystem>();
        _glimmer.Glimmer = glimmer;
    }

    [CommandImplementation("adjust")]
    public void Adjust(int delta)
    {
        _glimmer ??= GetSys<DVGlimmerSystem>();
        _glimmer.Glimmer += delta;
    }

    [CommandImplementation("get_entity")]
    public EntityUid Get(IInvocationContext ctx)
    {
        _glimmer ??= GetSys<DVGlimmerSystem>();
        if (_glimmer.Entity is not { } entity)
        {
            ctx.ReportError(new GlimmerMissingError());
            return EntityUid.Invalid;
        }

        return entity;
    }
}


public record struct GlimmerMissingError : IConError
{
    public readonly FormattedMessage DescribeInner()
    {
        return FormattedMessage.FromMarkupOrThrow("This command doesn't function if there's no glimmer. Is the round started?");
    }

    public string? Expression { get; set; }
    public Vector2i? IssueSpan { get; set; }
    public StackTrace? Trace { get; set; }
}
