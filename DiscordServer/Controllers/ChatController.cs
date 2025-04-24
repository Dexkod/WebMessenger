using ApplicationDiscord;
using ApplicationDiscord.Dto;
using DiscordDomain.Enums;
using DiscordDomain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiscordServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : Controller
{
    private ChatsDbContext _context;

    public ChatController(ChatsDbContext context)
    {
        _context = context;
    }


    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllMessage()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<ChatDto> result = new List<ChatDto>();

        var relationships = await _context.Relationships
            .AsNoTracking()
            .Include(_ => _.HistoryMessages)
            .Include(_ => _.Users)
            .Where(_ => _.Users.Any(u => u.Id == userId))
            .Select(_ =>
            new
            {
                User = _.Users.FirstOrDefault(_ => _.Id != userId) ?? _.Users.First(),
                HistoryMessages = _.HistoryMessages,
                Id = _.Id
            })
            .Select(u => new ChatDto()
            {
                Messages = u.HistoryMessages,
                PicturePath = $"/PictureStorage/{u.User!.PictureId}.jpg",
                Name = u.User.Login,
                RelationshipId = u.Id,
                ModifiedTime = u.HistoryMessages != null && u.HistoryMessages.Any() ? u.HistoryMessages.Max(_ => _.CreateAt) : DateTime.MinValue,
                IsGroup = false
            })
            .ToListAsync();

        var relationshipsGroup = await _context.Groups
            .AsNoTracking()
            .Include(_ => _.Groups)
            .Include(_ => _.HistoryMessages)
            .Where(_ => _.Groups!.Select(_ => _.UserId).Contains(userId))
            .Select(_ => new ChatDto
            {
                Messages = _.HistoryMessages,
                PicturePath = $"/PictureStorage/{_.PictureId}.jpg",
                Name = _.Name,
                RelationshipId = _.Id,
                ModifiedTime = _.HistoryMessages != null && _.HistoryMessages.Any() ? _.HistoryMessages.Max(_ => _.CreateAt) : DateTime.MinValue,
                IsGroup = true
            }).ToListAsync();

        result.AddRange(relationships);
        result.AddRange(relationshipsGroup);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateGroup(CreateGroupDto groupDto)
    {
        if (groupDto.UsersIds.Count < 2)
        {
            return BadRequest();
        }

        var groupDb = new Group()
        {
            Id = Guid.NewGuid(),
            PictureId = groupDto.PictureId.HasValue ? groupDto.PictureId.Value : Guid.Parse("190197f4-f672-4305-b587-8de083204592"),
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            Name = groupDto.Name
        };

        var list = new List<RelationshipGroup>();

        foreach (var id in groupDto.UsersIds)
        {
            list.Add(new RelationshipGroup()
            {
                Id = Guid.NewGuid(),
                GroupId = groupDb.Id,
                UserId = id,
                Role = GroupRole.User,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            });
        }

        list.Add(new RelationshipGroup()
        {
            Id = Guid.NewGuid(),
            GroupId = groupDb.Id,
            UserId = groupDto.CreatedUserId,
            Role = GroupRole.Admin,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        });

        _context.Groups.Add(groupDb);
        _context.RelationshipGroups.AddRange(list);
        //await _context.SaveChangesAsync();

        return Ok(groupDb.Id);
    }
}
