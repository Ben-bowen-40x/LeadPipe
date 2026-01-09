using LeadPipe.Infrastructure.Entity.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeadPipe.Infrastructure.MySql.Context.Configuration;

internal sealed class CustardMySqlEntityConfiguration(string schema)
        : IEntityTypeConfiguration<CustardMySqlEntity>
{
    private readonly string _schema = schema;

    public void Configure(EntityTypeBuilder<CustardMySqlEntity> entity)
    {
        entity.ToTable("customer", schema: _schema);
        entity.HasKey(x => x.customerID);

        entity.HasMany(c => c.subscriptions)
              .WithOne(s => s.customer)
              .HasForeignKey(s => s.customerID)
              .OnDelete(DeleteBehavior.NoAction);
    }
}
