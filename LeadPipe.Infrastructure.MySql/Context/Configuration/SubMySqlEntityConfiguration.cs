using LeadPipe.Infrastructure.Entity.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeadPipe.Infrastructure.MySql.Context.Configuration;

internal sealed class SubMySqlEntityConfiguration(string schema)
        : IEntityTypeConfiguration<SubMySqlEntity>
{
    private readonly string _schema = schema;

    public void Configure(EntityTypeBuilder<SubMySqlEntity> entity)
    {
        entity.ToTable("subscription", schema: _schema);
        entity.HasKey(x => x.subscriptionID);
    }
}
