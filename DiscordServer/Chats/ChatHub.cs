using ApplicationDiscord;
using DiscordDomain.Enums;
using DiscordDomain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DiscordServer.Chats;

public class ChatHub : Hub
{
    private ChatsDbContext _context;
    private static Dictionary<Guid, string> _connections = new Dictionary<Guid, string>();
    private static object locker = new();
    public ChatHub(ChatsDbContext context)
    {
        _context = context;
    }

    public void SetConnection(string connectionId, Guid id)
    {
        lock (locker)
        {
            if (string.IsNullOrEmpty(connectionId) || id == default)
            {
                return;
            }

            if (_connections.ContainsKey(id))
            {
                _connections[id] = connectionId;
            }
            else
            {
                _connections.Add(id, connectionId);
            }
        }
    }

    public async Task Send(string message, Guid userId, bool isGroup, Guid chatId)
    {
        List<string> connections = new List<string>();
        HistoryMessage historyMessage = null;
        
        if (isGroup)
        {
            historyMessage = new HistoryMessage
            {
                TextMessage = message,
                UserId = userId,
                RelationshipId = chatId,
                MessageType = MessageType.Text
            };

            var relationship = await _context.Relationships.Include(_ => _.Users)
                .Select(_ => new { Id = _.Id, Users = _.Users }).FirstAsync(_ => _.Id == chatId);

            foreach(var id in relationship.Users.Select(_ => _.Id))
            {
                if (!_connections.ContainsKey(id))
                {
                    continue;
                }

                connections.Add(_connections[id]);
            }
        }
        else
        {
            historyMessage = new HistoryMessage
            {
                TextMessage = message,
                UserId = userId,
                GroupId = chatId,
                MessageType = MessageType.Text
            };
        }
        await Clients.Clients(connections).SendAsync("Receive", message, userId, isGroup, chatId, historyMessage);
        //_context.HistoryMessages.Add(historyMessage);
        //await _context.SaveChangesAsync();
    }

    public async Task OfferCall(Guid chatId, Guid senderId, int typeCall)
    {
        var users = await _context.Relationships.Include(_ => _.Users)
                .Select(_ => new { Id = _.Id, Users = _.Users }).FirstAsync(_ => _.Id == chatId);
        var recipientId = users.Users.Select(_ => _.Id).Single(_ => _ != senderId);

        if (!_connections.ContainsKey(recipientId))
        {
            return;
        }

        await Clients.Client(_connections[recipientId]).SendAsync("OfferCall", senderId, chatId, typeCall);
    }

    public async Task UnTakeOffer(Guid senderId)
    {
        await Clients.Clients(_connections[senderId]).SendAsync("UnTakeOffer");
    }

    public async Task TakeOffer(string room, Guid senderId, Guid recipientId)
    {
        await Clients.Clients(_connections[senderId], _connections[recipientId]).SendAsync("GetCall", room);
    }

    public async Task ReceiveSignal(string signal)
    {
        await Clients.All.SendAsync("ReceiveSignal", signal);
    }
}
