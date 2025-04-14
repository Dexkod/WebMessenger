using DiscordDomain.Enums;

namespace ApplicationDiscord.Dto;

public record InfoUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; } = null!;
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public Guid PictureId { get; set; }
    public Status Status { get; set; }
    public StatusProfile StatusProfile { get; set; }
    public string Login { get; set; } = null!;
}
