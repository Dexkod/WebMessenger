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
        private IConfiguration _config;
        public UserController(ChatsDbContext context,
            IConfiguration config)
        {
            _config = config;
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

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdatePicture(IFormFile file)
        {
            var pictureId = Guid.NewGuid();
            
            await using (var fileStream = new FileStream($"{_config["StoragePath:Default"]}\\{pictureId}.jpg", FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var user = await _context.Users.FirstAsync(_ => _.Id == userId);

            user.PictureId = pictureId;
            await _context.SaveChangesAsync();

            return Ok(pictureId);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto updateUser)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FirstAsync(_ => _.Id == userId);


            user.Login = updateUser.Login ?? user.Login;
            user.Email = updateUser.Email ?? user.Email;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }


}
