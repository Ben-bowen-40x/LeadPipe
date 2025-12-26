using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace LeadPipe.Infrastructure.Sqlite.Context;

public sealed class SqlitePragmaInterceptor : DbConnectionInterceptor
{
    public override void ConnectionOpened(
        DbConnection connection,
        ConnectionEndEventData eventData)
    {
        if (connection is not SqliteConnection sqlite)
            return;

        using var cmd = sqlite.CreateCommand();
        cmd.CommandText = """
            PRAGMA journal_mode = WAL;
            PRAGMA synchronous = NORMAL;
            PRAGMA temp_store = MEMORY;
            PRAGMA foreign_keys = ON;
            PRAGMA cache_size = -200000;
            """;
        cmd.ExecuteNonQuery();
    }
}
