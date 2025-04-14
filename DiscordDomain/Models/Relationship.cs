using DiscordDomain.Enums;

namespace DiscordDomain.Models;

public record Relationship : BaseEntity
{
    public List<User> Users { get; set; } = new List<User>();
    public RelationshipStatus Status { get; set; }
    public List<HistoryMessage>? HistoryMessages { get; set; }
}
