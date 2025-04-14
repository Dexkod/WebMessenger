using DiscordDomain.Enums;

namespace DiscordDomain.Models;

public record RelationshipGroup : BaseEntity
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public GroupRole Role { get; set; }
}
