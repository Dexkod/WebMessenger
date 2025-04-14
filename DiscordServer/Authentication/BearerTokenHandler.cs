using ApplicationDiscord;
using DiscordDomain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DiscordServer.Authentication;

public class BearerTokenHandler : AuthenticationHandler<BearerTokenSchemeOptions>
{
    private ChatsDbContext _dbContext;

    public BearerTokenHandler(
        IOptionsMonitor<BearerTokenSchemeOptions> options,
        ILoggerFactory factory,
        UrlEncoder encoder,
        ChatsDbContext dbContext) : base(options, factory, encoder)
    {
        _dbContext = dbContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization
            .SingleOrDefault(_ => _ != null && _.StartsWith("Bearer "));

        if (authorization != null)
        {
            var token = authorization["Bearer ".Length..];

            string? role = null;
            User? userDb = null;

            if (Guid.TryParse(token, out var tokenId))
            {
                var tokenDb = await _dbContext.AuthorizationTokens
                    .Include(_ => _.User)
                    .SingleOrDefaultAsync(_ => _.Id == tokenId);

                if (tokenDb == null || tokenDb.CreateAt < DateTime.UtcNow.AddHours(-12))
                    return AuthenticateResult.Fail($"Expired token provided by: {Context.Connection.RemoteIpAddress}");
                
                userDb = tokenDb.User;
                role = nameof(Roles.User);
            }


            if (userDb == null || role == null)
                return AuthenticateResult.Fail($"Wrong token provided by: {Context.Connection.RemoteIpAddress}");

            return AuthenticateResult.Success(CreateTicket(token, userDb.Id, userDb.Login, role));
        }

        return AuthenticateResult.Fail($"Incorrect token provided by: {Context.Connection.RemoteIpAddress}");
    }

private AuthenticationTicket CreateTicket(string token, Guid userId, string login, string role)
{
    var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, token),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, role)
        };
    var identity = new ClaimsIdentity(claims, Scheme.Name);
    return new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
}
}

