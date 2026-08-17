using Content.Shared._DVA.DVCustomObjectiveSummary;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Network;

namespace Content.Client._DVA.DVCustomObjectiveSummary;

public sealed class DVCustomObjectiveSummaryUIController : UIController
{
    [Dependency] private readonly IClientNetManager _net = default!;

    private DVCustomObjectiveSummaryWindow? _window;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<DVCustomObjectiveSummaryOpenMessage>(OnDVCustomObjectiveSummaryOpen);
    }

    private void OnDVCustomObjectiveSummaryOpen(DVCustomObjectiveSummaryOpenMessage msg, EntitySessionEventArgs args)
    {
        OpenWindow();
    }

    public void OpenWindow()
    {
        // If a window is already open, close it
        _window?.Close();

        _window = new DVCustomObjectiveSummaryWindow();
        _window.OpenCentered();
        _window.OnClose += () => _window = null;
        _window.OnSubmitted += OnFeedbackSubmitted;
    }

    private void OnFeedbackSubmitted(string args)
    {
        var msg = new DVCustomObjectiveClientSetObjective
        {
            Summary = args,
        };
        _net.ClientSendMessage(msg);
        _window?.Close();
    }
}
