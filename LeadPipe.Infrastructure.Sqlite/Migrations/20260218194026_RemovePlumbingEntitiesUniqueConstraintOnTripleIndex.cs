using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadPipe.Infrastructure.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlumbingEntitiesUniqueConstraintOnTripleIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Date_Source",
                table: "PlumbingEntities");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_Date",
                table: "PlumbingEntities",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Date_Source",
                table: "PlumbingEntities",
                columns: new[] { "PhoneNumber", "Date", "Source" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlumbingEntities_Date",
                table: "PlumbingEntities");

            migrationBuilder.DropIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Date_Source",
                table: "PlumbingEntities");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Date_Source",
                table: "PlumbingEntities",
                columns: new[] { "PhoneNumber", "Date", "Source" },
                unique: true);
        }
    }
}
