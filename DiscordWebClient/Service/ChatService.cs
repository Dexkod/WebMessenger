using Microsoft.AspNetCore.SignalR.Client;

namespace DiscordWebClient.Service;

public class ChatService
{ 
    public Dictionary<Guid, HubConnection> Connects = new Dictionary<Guid, HubConnection>();
    public SemaphoreSlim semaphore = new SemaphoreSlim(1);

    public HubConnection GetHub(Guid id)
    {
        if (Connects.ContainsKey(id))
        {
            return Connects[id];
        }

        var hub = new HubConnectionBuilder()
            .WithUrl("http://localhost:5237/chat")
            .WithAutomaticReconnect()
            .Build();

        return hub;
    }

    public async Task StartHub(HubConnection hub, Guid userId)
    {
        try
        {
            await semaphore.WaitAsync();
            await hub.StartAsync();
            await hub.InvokeAsync("SetConnection", hub.ConnectionId, userId);

            if (!Connects.ContainsKey(userId))
            {
                Connects.Add(userId, hub);
            }
        }
        finally
        {
            semaphore.Release();
        }
    }
}
