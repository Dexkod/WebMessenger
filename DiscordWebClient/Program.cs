using ApplicationDiscord;
using DiscordWebClient.Authentication;
using DiscordWebClient.Components;
using DiscordWebClient.Service;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazorBootstrap();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "BearerTokenSchemeOptions";
        options.DefaultChallengeScheme = "BearerTokenSchemeOptions";
    })
    .AddScheme<BearerTokenSchemeOptions, BearerTokenHandler>("BearerTokenSchemeOptions", options => { });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContext<ChatsDbContext>(
        o => o.UseNpgsql(builder.Configuration.GetConnectionString("ChatDb")));
builder.Services.AddControllers();
builder.Services.AddScoped<WebRtcService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider("C:\\CSMy\\Disscord\\ApplicationDiscord\\PictureStorage"),
    RequestPath = new PathString("/PictureStorage")
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();

app.Run();