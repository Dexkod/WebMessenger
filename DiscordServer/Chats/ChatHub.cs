using ApplicationDiscord;
using DiscordDomain.Enums;
using DiscordDomain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DiscordServer.Chats;

public class ChatHub : Hub
{
    private ChatsDbContext _context;

    public ChatHub(ChatsDbContext context)
    {
        _context = context;
    }

    public async Task Send(string message, Guid userId, bool isGroup, Guid chatId)
    {
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

        _context.HistoryMessages.Add(historyMessage);

        await _context.SaveChangesAsync();

        await Clients.All.SendAsync("Receive", message, userId, isGroup, chatId, historyMessage);
    }

    public async Task ReceiveSignal(string signal)
    {
        await Clients.All.SendAsync("ReceiveSignal", signal);
    }
}
