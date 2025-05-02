using DiscordDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationDiscord;

public class ChatsDbContext : DbContext 
{
    public ChatsDbContext(DbContextOptions options) : base(options){}

    public DbSet<User> Users { get; set; }
    public DbSet<AuthorizationToken> AuthorizationTokens { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<HistoryMessage> HistoryMessages { get; set; }
    public DbSet<Relationship> Relationships { get; set; }
    public DbSet<RelationshipGroup> RelationshipGroups { get; set; }
    public DbSet<Notification> Notifications { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Chat;Username=postgres;Password=1111;Include Error Detail=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Relationship>()
            .HasMany(r => r.Users)
            .WithMany(u => u.Relationships);

        modelBuilder.Entity<AuthorizationToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.AuthorizationToken)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HistoryMessage>()
            .Property(nameof(HistoryMessage.UserId))
            .IsRequired();
    }
}
