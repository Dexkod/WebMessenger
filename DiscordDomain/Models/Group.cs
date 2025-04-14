namespace DiscordDomain.Models;

public record Group : BaseEntity
{
    public string Name { get; set; } = null!;
    public Guid PictureId { get; set; }

    public List<HistoryMessage>? HistoryMessages { get; set; }
    public List<RelationshipGroup>? Groups { get; set; }
}
