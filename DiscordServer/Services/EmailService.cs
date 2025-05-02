using MimeKit;
using MailKit.Net.Smtp;
using ApplicationDiscord;
using DiscordDomain.Models;
using DiscordDomain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DiscordServer.Services;

public class EmailService
{
    private ChatsDbContext _context;
    private IConfiguration _configuration;

    public EmailService(ChatsDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;

    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Администрация сайта", _configuration["EmailService:login"]));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };

        using (var client = new SmtpClient())
        {
            client.Connect("smtp.mail.ru", 587, false);
            await client.AuthenticateAsync(_configuration["EmailService:login"], _configuration["EmailService:password"]);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }

        _context.Notifications.Add(new Notification()
        {
            Type = NotificationType.Email,
            Message = message,
            Addressee = email,
            Subject = subject,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TryFindMessage(Notification notification)
    {
        var result = await _context.Notifications
            .FirstOrDefaultAsync(_ => _.Type == NotificationType.Email && _.Addressee == notification.Addressee
             && _.Message != null && _.Message.Contains(notification.Message!));

        return result == null ? false : true;
    }
}
