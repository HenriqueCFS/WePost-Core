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
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        base.OnModelCreating(builder);
    }
}