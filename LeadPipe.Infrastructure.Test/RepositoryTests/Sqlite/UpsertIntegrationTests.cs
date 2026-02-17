using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Sqlite.Context;
using LeadPipe.Infrastructure.Sqlite.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LeadPipe.Infrastructure.Test.RepositoryTests.Sqlite;

public class UpsertIntegrationTests
{
    private readonly DbContextOptions<PlumbingContext> _options;

    public UpsertIntegrationTests()
    {
        // 1. Build configuration to read secrets.json
        var config = new ConfigurationBuilder()
            .AddUserSecrets("f61f648e-c555-4854-ab9a-40533affb5d6") // 'Program' is in your main project
            .Build();

        // 2. Get the "Plumbing" string
        var connectionString = config.GetConnectionString("PlumbingTest");

        // 3. Setup Options
        _options = new DbContextOptionsBuilder<PlumbingContext>()
            .UseSqlite(connectionString)
            .Options;
    }

    [Fact]
    public async Task Upsert_ShouldHandleDuplicateKeysBySelectingMinDate()
    {
        // Arrange
        using var context = new PlumbingContext(_options);
        var logger = Substitute.For<ILogger<PlumbingCaliperLinkRepository>>();
        PlumbingCaliperLinkRepository repo = new(context, logger);
        await context.Database.EnsureCreatedAsync(); // Double check the schema exists

        var testData = new List<PlumbingCaliperLink>
        {
            new() { PlumbingId = 1, CaliperId = 1, MatchingPhone = 555, UnixMatchDate = 1000 },
            new() { PlumbingId = 1, CaliperId = 1, MatchingPhone = 555, UnixMatchDate = 500 } // This should win
        };

        // Act
        var result = await repo.LinkUpsertAsync(testData, CancellationToken.None);

        // Assert
        var saved = context.PlumbingCaliperLinks.Single(x => x.PlumbingId == 1);
        Assert.Equal(500, saved.UnixMatchDate);
    }
}
