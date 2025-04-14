using ApplicationDiscord;
using ApplicationDiscord.Dto;
using DiscordDomain.Enums;
using DiscordDomain.Models;
using DiscordWebClient.MapperConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiscordServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private ChatsDbContext _context;

        public UserController(ChatsDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUser([FromQuery] string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            }

            var user = await _context.Users.FirstAsync(_ => _.Id.ToString() == id);
            var mapper = MapperConfig.InitializeAutomapper();

            return Ok(mapper.Map<User, InfoUser>(user));
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult GetUserId()
        {
            return Ok(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
        }
    }


}
