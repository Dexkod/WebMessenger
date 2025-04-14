namespace ApplicationDiscord.Dto;

public record UserToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = null!;
}
