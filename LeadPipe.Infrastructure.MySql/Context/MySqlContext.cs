using LeadPipe.Infrastructure.Entity.MySql;
using Microsoft.EntityFrameworkCore;

namespace LeadPipe.Infrastructure.MySql.Context;

public class MySqlReadonlyContext(DbContextOptions<MySqlReadonlyContext> options)
    : DbContext(options)
{
    public DbSet<CustomerMySqlEntity> Customers { get; set; }
    public DbSet<SubMySqlEntity> Subscriptions { get; set; }
    public DbSet<CallMySqlEntity> Calls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer
        var cust = modelBuilder.Entity<CustomerMySqlEntity>();
        cust.ToTable("customers");
        cust.HasKey(c => c.customerID);

        cust.Property(c => c.phone1).HasMaxLength(50);
        cust.Property(c => c.phone2).HasMaxLength(50);

        cust.HasMany(c => c.Subscriptions)
            .WithOne(s => s.Customer)
            .HasForeignKey(s => s.customerID);

        // Subscription
        var sub = modelBuilder.Entity<SubMySqlEntity>();
        sub.ToTable("subscriptions");
        sub.HasKey(s => s.subscriptionID);

        sub.HasIndex(s => s.customerID);

        // Calls
        var call = modelBuilder.Entity<CallMySqlEntity>();
        call.ToTable("calls");
        call.HasKey(c => c.call_id);

        call.HasIndex(c => c.contact_number_clean);
        call.HasIndex(c => c.called_at_utc);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Prevent accidental writes
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    public override int SaveChanges() =>
        throw new InvalidOperationException("Write operations are not allowed on read-only MySQL context.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("Write operations are not allowed on read-only MySQL context.");

}
