using LeadPipe.Infrastructure.MySql.Context;
using LeadPipe.Infrastructure.MySql.Repository;
using LeadPipe.Infrastructure.MySql.Settings;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace LeadPipe.Infrastructure.Test.RepositoryTests.MySql;

public class CustardMySqlRepositoryTests
{
    private CustardMySqlRepository CreateRepository()
    {
        var settings = Substitute.For<IMySqlSettings>();
        settings.Schema1.Returns("dbo");
        settings.Schema2.Returns("dbo");

        var context = new MySqlSchema1Context(
            new DbContextOptionsBuilder<MySqlSchema1Context>()
                .UseInMemoryDatabase(nameof(CustardMySqlRepositoryTests))
                .Options,
            settings);
        return new CustardMySqlRepository(context);
    }

   
}
