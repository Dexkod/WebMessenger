using DiscordDomain.Models;

namespace ApplicationDiscord.Dto;

public class ChatDto
{
    public string Name { get; set; } = null!;
    public List<HistoryMessage>? Messages { get; set; }
    public string PicturePath { get; set; } = null!;
    public Guid RelationshipId { get; set; }
}
