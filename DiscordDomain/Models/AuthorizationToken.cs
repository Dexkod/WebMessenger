namespace DiscordDomain.Models;

public record AuthorizationToken : BaseEntity
{
    public Guid Token { get; set; }
    public DateTime EndTime { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
