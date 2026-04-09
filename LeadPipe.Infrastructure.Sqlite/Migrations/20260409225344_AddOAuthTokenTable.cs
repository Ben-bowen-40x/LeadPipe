using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadPipe.Infrastructure.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddOAuthTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OAuthTokens",
                columns: table => new
                {
                    Provider = table.Column<string>(type: "TEXT", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    TokenType = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UnixExpiresAtUtc = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UnixUpdatedAtUtc = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthTokens", x => x.Provider);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthTokens");
        }
    }
}
