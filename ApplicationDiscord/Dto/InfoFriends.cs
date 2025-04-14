using DiscordDomain.Enums;

namespace ApplicationDiscord.Dto;

public class InfoFriends
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Status Status { get; set; }
    public Guid PictureId { get; set; }
    public bool IsFriend { get; set; }
    public string PicturePath { get; set; }
}
