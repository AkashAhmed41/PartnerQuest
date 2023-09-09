namespace BackendWebApi.SignalR;

public class ActiveStatusTracker
{
    private static readonly Dictionary<string, List<string>> ActiveUsers = new Dictionary<string, List<string>>();

    public Task UserIsConnected (string username, string connectionId)
    {
        lock(ActiveUsers)
        {
            if (ActiveUsers.ContainsKey(username))
            {
                ActiveUsers[username].Add(connectionId);
            }
            else
            {
                ActiveUsers.Add(username, new List<string>{connectionId});
            }
        }

        return Task.CompletedTask;
    }

    public Task UserIsDisconnected (string username, string connectionId) 
    {
        lock(ActiveUsers)
        {
            if (!ActiveUsers.ContainsKey(username)) return Task.CompletedTask;
            ActiveUsers[username].Remove(connectionId);

            if (ActiveUsers[username].Count == 0)
            {
                ActiveUsers.Remove(username);
            }
        }

        return Task.CompletedTask;
    }

    public Task<string[]> GetActiveUsers()
    {
        string[] currentlyActiveUsers;
        lock(ActiveUsers)
        {
            currentlyActiveUsers = ActiveUsers.OrderBy(u => u.Key).Select(u => u.Key).ToArray();
        }

        return Task.FromResult(currentlyActiveUsers);
    }
}