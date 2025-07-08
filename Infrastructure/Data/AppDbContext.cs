using consware_api.Domain.Entities;
using consware_api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace consware_api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.password)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.role)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.created_at)
                .IsRequired();

            entity.Property(e => e.updated_at)
                .IsRequired(false);

            entity.Property(e => e.active)
                .IsRequired()
                .HasDefaultValue(true);

            entity.HasIndex(e => e.email)
                .IsUnique();

            entity.ToTable("Users");
        });

        base.OnModelCreating(modelBuilder);
    }
}
