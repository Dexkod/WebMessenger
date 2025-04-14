using DiscordDomain.Enums;

namespace DiscordDomain.Models;

public record User : BaseEntity
{
    public string Login { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Guid PictureId { get; set; }
    public Status Status { get; set; }
    public StatusProfile StatusProfile { get; set; }

    public List<Relationship>? Relationships { get; set; }
    public List<RelationshipGroup>? Groups { get; set; }
    public List<AuthorizationToken>? AuthorizationToken { get; set; }
}
