using DiscordDomain.Enums;

namespace DiscordDomain.Models;

public record Notification : BaseEntity
{
    public NotificationType Type  { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
    public string? Addressee { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
