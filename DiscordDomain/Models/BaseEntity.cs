namespace DiscordDomain.Models;

public record BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}
