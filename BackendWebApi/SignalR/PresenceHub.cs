using BackendWebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BackendWebApi.SignalR;

[Authorize]
public class PresenceHub : Hub
{
    private readonly ActiveStatusTracker _activeStatusTracker;
    public PresenceHub(ActiveStatusTracker activeStatusTracker)
    {
        _activeStatusTracker = activeStatusTracker;        
    }
    public override async Task OnConnectedAsync()
    {
        await _activeStatusTracker.UserIsConnected(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        var currentlyActiveUsers = await _activeStatusTracker.GetActiveUsers();
        await Clients.All.SendAsync("GetActiveUsers", currentlyActiveUsers);
    }
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await _activeStatusTracker.UserIsDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

        var currentlyActiveUsers = await _activeStatusTracker.GetActiveUsers();
        await Clients.All.SendAsync("GetActiveUsers", currentlyActiveUsers);

        await base.OnDisconnectedAsync(exception);
    }
}