using ApplicationDiscord;
using ApplicationDiscord.Dto;
using DiscordDomain.Enums;
using DiscordDomain.Models;
using DiscordServer.Authentication;
using DiscordServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiscordServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthorizationController : ControllerBase
{
    private ChatsDbContext _context;
    private EmailService _emailService;

    public AuthorizationController(ChatsDbContext context,
        EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> SendCode(ApprovalAccount approvalAccount)
    {
        var number = (new Random()).Next(1000, 10000);
        string text = $"Код подтверждения акаунта {approvalAccount.Login}: {number}";
        await _emailService.SendEmailAsync(approvalAccount.Email, "Регистрация аккаунта", text);

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> SignIn([FromBody] SignInUser user)
    {
        var userToken = await GetUserToken(user);

        if (userToken == null)
        {
            BadRequest();
        }

        return Ok(userToken);
    }

    [AllowAnonymous]
    [HttpDelete("[action]")]
    public async Task<IActionResult> LogOut()
    {
        var token = User.FindFirst(ClaimTypes.Sid)!.Value;
        var id = Guid.Parse(token);

        var tokenDb = await _context.AuthorizationTokens.FirstAsync(_ => _.Id == id);
        _context.AuthorizationTokens.Remove(tokenDb);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterUser user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userDb = await _context.Users.FirstOrDefaultAsync(_ => _.Login == user.Login);
        var checkEmail = await _emailService.TryFindMessage(new Notification()
        {
            Addressee = user.Email,
            Type = NotificationType.Email,
            Message = user.Code
        });

        if (userDb != null)
        {
            return Conflict();
        }

        if (!checkEmail)
        {
            return BadRequest();
        }

        var salt = Hash.GeneratePasswordSalt();
        var passwordHash = Hash.StringWithSalt(user.Password, salt);

        var newUser = new User()
        {
            Id = new Guid(),
            Login = user.Login,
            PasswordSalt = salt,
            PasswordHash = passwordHash,
            Phone = user.Phone,
            Email = user.Email,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Gender = user.Gender,
            Age = user.Age,
            PictureId = FileStorage.DefaultPictureId,
            Status = Status.Inactive,
            StatusProfile = StatusProfile.Open
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var userToken = await GetUserToken(new SignInUser()
        {
            Login = user.Login,
            Password = user.Password
        });

        return Ok(userToken);
    }

    private async Task<UserToken> GetUserToken(SignInUser user)
    {
        var userDb = await _context.Users.SingleOrDefaultAsync(_ => _.Login == user.Login.Trim());

        if (userDb == null)
        {
            return null!;
        }

        if (userDb.PasswordHash != Hash.StringWithSalt(user.Password, userDb.PasswordSalt))
        {
            return null!;
        }

        var authToken = new AuthorizationToken()
        {
            Id = new Guid(),
            UserId = userDb.Id,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
        };

        _context.AuthorizationTokens.Add(authToken);
        await _context.SaveChangesAsync();

        return new UserToken()
        {
            Id = userDb.Id,
            Token = authToken.Id.ToString()
        };
    }
}
