using ApplicationDiscord;
using ApplicationDiscord.Dto;
using DiscordDomain.Enums;
using DiscordDomain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

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
            .Select(u =>  new ChatDto()
            {
                Messages = u.HistoryMessages,
                PicturePath = $"/PictureStorage/{u.User!.PictureId}.jpg",
                Name = u.User.Login,
                RelationshipId = u.Id
            })
            .ToListAsync();

        result.AddRange(relationships);

        return Ok(result);
    }
}
