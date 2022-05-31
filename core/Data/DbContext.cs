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

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Friend> Friends { get; set; } = null!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

        builder.Entity<Friend>()
            .HasOne(a => a.RequestedBy)
            .WithMany(b => b.SentFriendRequests)
            .HasForeignKey(c => c.RequestedById);

        builder.Entity<Friend>()
            .HasOne(a => a.RequestedTo)
            .WithMany(b => b.ReceievedFriendRequests)
            .HasForeignKey(c => c.RequestedToId);

        builder.Entity<Friend>()
            .HasKey(f => new { f.RequestedById, f.RequestedToId });
    }
}