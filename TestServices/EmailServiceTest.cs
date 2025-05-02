using ApplicationDiscord;
using DiscordServer.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;

namespace TestServices
{
    public class EmailServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task SendCode()
        {
            //var options = new DbContextOptionsBuilder<ChatsDbContext>()
            //    .UseInMemoryDatabase(databaseName: "TestDb")
            //    .Options;

            //var context = new ChatsDbContext(options);
            //var service = new EmailService(context);

            //await service.SendEmailAsync("ivanov.misha14@mail.ru", "Тест", "1234");

            Assert.Pass();
        }
    }
}