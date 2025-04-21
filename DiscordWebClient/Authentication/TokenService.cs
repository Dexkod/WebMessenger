using ApplicationDiscord;
using Microsoft.EntityFrameworkCore;

namespace DiscordWebClient.Authentication;

public class TokenService
{
    private readonly ChatsDbContext _context;

    public TokenService(ChatsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var tokenEntity = await _context.AuthorizationTokens
            .FirstOrDefaultAsync(t => t.Id.ToString() == token && t.CreateAt > DateTime.UtcNow.AddHours(-12));

        return tokenEntity != null;
    }

    public async Task<UserInfo> GetUserInfoAsync(string token)
    {
        var tokenEntity = await _context.AuthorizationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id.ToString() == token);

        if (tokenEntity == null) return null;

        return new UserInfo
        {
            UserId = tokenEntity.User.Id,
            UserName = tokenEntity.User.Login,
            Token = token
        };
    }
}
