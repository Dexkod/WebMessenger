using ApplicationDiscord;
using DiscordServer.Authentication;
using DiscordServer.Chats;
using DiscordServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ChatsDbContext>(
        o => o.UseNpgsql(builder.Configuration.GetConnectionString("ChatDb")));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "BearerTokenSchemeOptions";
        options.DefaultChallengeScheme = "BearerTokenSchemeOptions";
    })
    .AddScheme<BearerTokenSchemeOptions, BearerTokenHandler>("BearerTokenSchemeOptions", options => { });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddScoped<EmailService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapHub<MessageHub>("/message");

app.Run();
