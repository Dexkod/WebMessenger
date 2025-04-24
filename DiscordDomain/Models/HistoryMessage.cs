using DiscordDomain.Enums;

namespace DiscordDomain.Models;

public record HistoryMessage : BaseEntity
{
    public ChatType ChatType { get; set; }
    public MessageType MessageType { get; set; }
    public string? TextMessage { get; set; }
    public Guid? MessageId { get; set; }
    public string UserName { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
    public Guid? RelationshipId { get; set; }
    public Relationship? Relationship { get; set; }
}
