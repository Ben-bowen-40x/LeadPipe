using LeadPipe.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace LeadPipe.Infrastructure.Database;

internal class PlumbingContext(DbContextOptions<PlumbingContext> options) : DbContext(options)
{
    public DbSet<SubsEntity> SubsEntities { get; set; }
    public DbSet<PlumbingEntity> PlumbingEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SubsEntity configuration
        var sub = modelBuilder.Entity<SubsEntity>();
        sub.HasKey(s => s.Id);
        sub.HasIndex(s => s.PhoneNumber);

        // PlumbingEntity configuration
        var plumb = modelBuilder.Entity<PlumbingEntity>();
        plumb.HasKey(p => p.Id);
        plumb.HasIndex(p => p.PhoneNumber);
        plumb.Property(p => p.Source)
            .HasConversion<string>();

        // Many-to-many relationship
        modelBuilder.Entity<SubsEntity>()
            .HasMany(p => p.PlumbingEntities)
            .WithMany(s => s.SubsEntities)
            .UsingEntity<Dictionary<string, object>>(
                "SubsPlumbing",
                j => j.HasOne<PlumbingEntity>()
                      .WithMany()
                      .HasForeignKey("PlumbingEntityId")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<SubsEntity>()
                      .WithMany()
                      .HasForeignKey("SubsEntityId")
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("SubsEntityId", "PlumbingEntityId");
                    j.HasIndex("SubsEntityId");
                    j.HasIndex("PlumbingEntityId");
                });
    }
}