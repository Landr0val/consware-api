using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace TravelRequests.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TravelRequest> TravelRequests { get; set; } = null!;
    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; } = null!;

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

        modelBuilder.Entity<TravelRequest>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.user_id)
                .IsRequired();

            entity.Property(e => e.origin_city)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.destination_city)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.departure_date)
                .IsRequired();

            entity.Property(e => e.return_date)
                .IsRequired();

            entity.Property(e => e.justification)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.created_at)
                .IsRequired();

            entity.HasOne(e => e.user)
                .WithMany(u => u.travel_requests)
                .HasForeignKey(e => e.user_id);

            entity.ToTable("TravelRequests");
        });

        modelBuilder.Entity<PasswordResetCode>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.user_id)
                .IsRequired();

            entity.Property(e => e.code)
                .IsRequired()
                .HasMaxLength(6);

            entity.Property(e => e.email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.created_at)
                .IsRequired();

            entity.Property(e => e.expires_at)
                .IsRequired();

            entity.Property(e => e.used)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.used_at)
                .IsRequired(false);

            entity.HasOne(e => e.user)
                .WithMany()
                .HasForeignKey(e => e.user_id);

            entity.ToTable("PasswordResetCodes");
        });

        base.OnModelCreating(modelBuilder);
    }
}
