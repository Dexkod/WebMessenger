namespace ApplicationDiscord.Dto;

public record SignInUser
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}
