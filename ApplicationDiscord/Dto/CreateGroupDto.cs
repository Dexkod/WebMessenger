namespace ApplicationDiscord.Dto;

public class CreateGroupDto
{
    public string Name { get; set; } = null!;
    public Guid CreatedUserId { get; set; }
    public List<Guid> UsersIds { get; set; } = null!;
    public Guid? PictureId { get; set; }
}
