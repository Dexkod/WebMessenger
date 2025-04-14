using DiscordDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApplicationDiscord.Dto;

public record RegisterUser
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
    public string Email { get; set; } = null!;
    //[RegularExpression(@"[7[0-9]{10}]", ErrorMessage = "Некорректный телефон")]
    public string Phone { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; } = null!;
    public Gender Gender { get; set; }
    public int Age { get; set; }
}
