using DiscordDomain.Models;
using System.Collections.Concurrent;

namespace ApplicationDiscord.Dto;

public class ChatDto
{
    public string Name { get; set; } = null!;
    public ConcurrentQueue<HistoryMessage>? Messages { get; set; }
    public string PicturePath { get; set; } = null!;
    public Guid RelationshipId { get; set; }
    public DateTime ModifiedTime { get; set; }
    public bool IsGroup { get; set; }
}
