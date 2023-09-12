namespace BackendWebApi.SignalR;

public class ActiveStatusTracker
{
    private static readonly Dictionary<string, List<string>> ActiveUsers = new Dictionary<string, List<string>>();

    public Task<bool> UserIsConnected (string username, string connectionId)
    {
        bool userIsConnected = false;
        lock(ActiveUsers)
        {
            if (ActiveUsers.ContainsKey(username))
            {
                ActiveUsers[username].Add(connectionId);
            }
            else
            {
                ActiveUsers.Add(username, new List<string>{connectionId});
                userIsConnected = true;
            }
        }

        return Task.FromResult(userIsConnected);
    }

    public Task<bool> UserIsDisconnected (string username, string connectionId) 
    {
        bool userIsDisconnected = false;
        lock(ActiveUsers)
        {
            if (!ActiveUsers.ContainsKey(username)) return Task.FromResult(userIsDisconnected);
            ActiveUsers[username].Remove(connectionId);

            if (ActiveUsers[username].Count == 0)
            {
                ActiveUsers.Remove(username);
                userIsDisconnected = true;
            }
        }

        return Task.FromResult(userIsDisconnected);
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

    public static Task<List<string>> GetUserConnections (string username)
    {
        List<string> connectionIds;
        lock(ActiveUsers)
        {
            connectionIds = ActiveUsers.GetValueOrDefault(username);
        }

        return Task.FromResult(connectionIds);
    }
}