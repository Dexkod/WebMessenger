using ApplicationDiscord;
using ApplicationDiscord.Dto;
using ApplicationDiscord.Dto.Relationship;
using DiscordDomain.Enums;
using DiscordDomain.Models;
using DiscordWebClient.MapperConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace DiscordServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelationshipController : Controller
{
    private ChatsDbContext _context;

    public RelationshipController(ChatsDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> AddFriend([FromBody] AddFriendDto friend)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FirstAsync(_ => _.Id == Guid.Parse(userId!));
        var friendDb = await _context.Users.FirstAsync(_ => _.Id == friend.Id);

        var oldRelationship = await _context.Relationships
            .Include(_ => _.Users)
            .FirstOrDefaultAsync(_ => _.Users.Select(u => u.Id).Contains(user.Id) && _.Users.Select(u => u.Id).Contains(friendDb.Id));

        if (oldRelationship  != null)
        {
            oldRelationship.Status = RelationshipStatus.Friends;
            await _context.SaveChangesAsync();
            return Ok();
        }

        var relationship = new Relationship()
        {
            Users = new List<User>() { user, friendDb },
            Status = RelationshipStatus.Friends,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };

        _context.Relationships.Add(relationship);
        
        await _context.SaveChangesAsync();

        return Ok();
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetFriends([FromQuery] string login = "")
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var friendsDb = await _context.Relationships
            .Include(_ => _.Users)
            .Where(_ => _.Status == RelationshipStatus.Friends && _.Users.Select(u => u.Id).Contains(userId))
            .Select(_ => _.Users.FirstOrDefault(u => u.Id != userId))
            .ToListAsync();

        var mapper = MapperConfig.InitializeAutomapper();

        return Ok(friendsDb.Select(_ => mapper.Map<User, InfoFriends>(_)));
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetOtherPeople([FromQuery] string login = "")
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var otherPeople = await _context.Users
            .Include(_ => _.Relationships)
            .Where(_ => _.Id != userId && 
            (!_.Relationships.Where(r => r.Status == RelationshipStatus.Friends).Any(r => r.Users.Select(_ => _.Id).Contains(userId))))
            .Take(5)
            .ToListAsync();

        var mapper = MapperConfig.InitializeAutomapper();

        return Ok(otherPeople.Select(_ => mapper.Map<User, InfoFriends>(_)));
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> RemoveFriend([FromBody] AddFriendDto friend)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var relationship = await _context.Relationships
            .FirstAsync(_ => _.Users.Any(u => u.Id == userId) && _.Users.Any(u => u.Id == friend.Id));
        relationship.Status = RelationshipStatus.Default;
        _context.Relationships.Update(relationship);

        await _context.SaveChangesAsync();

        return Ok();
    }
}

