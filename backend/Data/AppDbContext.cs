using JobTracker.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<JobApplication> Applications => Set<JobApplication>();
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<JobApplication>(entity =>
        {
            entity.Property(a => a.Salary).HasColumnType("decimal(18,2)");
            entity.HasIndex(a => a.UserId);
            entity.HasIndex(a => a.Status);

            entity.HasOne(a => a.User)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.StatusHistories)
                .WithOne(h => h.Application)
                .HasForeignKey(h => h.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Contacts)
                .WithOne(c => c.Application)
                .HasForeignKey(c => c.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Reminders)
                .WithOne(r => r.Application)
                .HasForeignKey(r => r.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Documents)
                .WithOne(d => d.Application)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
