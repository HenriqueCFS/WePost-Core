using Microsoft.EntityFrameworkCore;
using core.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

public class ProjectContext : IdentityDbContext, IDataProtectionKeyContext
{
    public ProjectContext(DbContextOptions<ProjectContext> options)
        : base(options)
    {
        
    }

    public DbSet<AppUser> Users { get; set; } = null!;
    public DbSet<Friend> Friends { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .HasMany(f => f.ReceievedFriendRequests)
            .WithOne(f => f.RequestedTo)
            .HasForeignKey(f => f.RequestedToId);

        builder.Entity<AppUser>()
            .HasMany(f => f.SentFriendRequests)
            .WithOne(f => f.RequestedBy)
            .HasForeignKey(f => f.RequestedById);


        builder.Entity<AppUser>()
            .HasMany(f => f.SentMessages)
            .WithOne(f => f.Sender)
            .HasForeignKey(f => f.SenderId);

        builder.Entity<AppUser>()
            .HasMany(f => f.ReceivedMessages)
            .WithOne(f => f.Receiver)
            .HasForeignKey(f => f.ReceiverId);

        builder.Entity<ChatMessage>()
            .HasOne(a => a.Sender)
            .WithMany(b => b.SentMessages)
            .HasForeignKey(c => c.SenderId)
            .IsRequired(); 

        builder.Entity<ChatMessage>()
            .HasOne(a => a.Receiver)
            .WithMany(b => b.ReceivedMessages)
            .HasForeignKey(c => c.ReceiverId)
            .IsRequired();


        builder.Entity<Friend>()
            .HasOne(a => a.RequestedBy)
            .WithMany(b => b.SentFriendRequests)
            .HasForeignKey(c => c.RequestedById)
            .IsRequired();

        builder.Entity<Friend>()
            .HasOne(a => a.RequestedTo)
            .WithMany(b => b.ReceievedFriendRequests)
            .HasForeignKey(c => c.RequestedToId)
            .IsRequired();

        builder.Entity<Friend>()
            .HasKey(f => new { f.RequestedById, f.RequestedToId });
    }
}