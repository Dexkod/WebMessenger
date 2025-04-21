using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DiscordWebClient.Authentication;

public class BearerTokenHandler : AuthenticationHandler<BearerTokenSchemeOptions>
{
    public BearerTokenHandler(
        IOptionsMonitor<BearerTokenSchemeOptions> options,
        ILoggerFactory factory,
        UrlEncoder encoder) : base(options, factory, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return AuthenticateResult.Success(CreateTicket("", Guid.NewGuid(), "", nameof(Roles.Administrator)));
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

