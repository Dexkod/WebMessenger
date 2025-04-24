using DiscordDomain.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace DiscordWebClient.Service;

public class ChatService
{ 
    public static Dictionary<Guid, HubConnection> Connects = new Dictionary<Guid, HubConnection>();
    public SemaphoreSlim semaphore = new SemaphoreSlim(1);
    public event Func<string, Guid, Guid, HistoryMessage, Task>? OnMessageReceived;
    public event Func<Guid, Guid, int, Task>? OnOfferCallReceived;
    public event Action<string>? OnGetCallReceived;
    public event Func<Task>? OnUnTakeOfferReceived;
    public event Action<string, string, Guid>? OnCreateChatReceived;

    public HubConnection GetHub(Guid id)
    {
        if (Connects.ContainsKey(id) && Connects[id].State != HubConnectionState.Disconnected)
        {
            return Connects[id];
        }

        var hub = new HubConnectionBuilder()
            .WithUrl("http://localhost:5237/chat")
            .WithAutomaticReconnect()
            .Build();

        hub.On<string, Guid, Guid, HistoryMessage>("Receive", async (a, b, d, e) => await OnMessageReceived?.Invoke(a, b, d, e));
        hub.On<Guid, Guid, int>("OfferCall", async (a, b, c) => await OnOfferCallReceived?.Invoke(a, b, c));
        hub.On<string>("GetCall", (a) => OnGetCallReceived?.Invoke(a));
        hub.On("UnTakeOffer", async () => await  OnUnTakeOfferReceived?.Invoke());
        hub.On<string, string, Guid>("CreateChat", (a, b, c) => OnCreateChatReceived?.Invoke(a, b, c));

        return hub;
    }

    public async Task StartHub(HubConnection hub, Guid userId)
    {
        try
        {
            await semaphore.WaitAsync();

            if (!Connects.ContainsKey(userId))
            {
                await hub.StartAsync();
                await hub.InvokeAsync("SetConnection", hub.ConnectionId, userId);
                Connects.Add(userId, hub);
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

}
