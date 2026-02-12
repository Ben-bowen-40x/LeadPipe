using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadPipe.Infrastructure.Sqlite.Migrations.Plumbing
{
    /// <inheritdoc />
    public partial class RemoveCaliperPhoneDateUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Source",
                table: "PlumbingEntities");

            migrationBuilder.DropIndex(
                name: "IX_CornEntities_PhoneNumber_Source",
                table: "CornEntities");

            migrationBuilder.DropIndex(
                name: "IX_CaliperEntities_PhoneNumber_Date",
                table: "CaliperEntities");

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "SandPlumbingLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "SandCornLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "SandCaliperLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "PlumbingCaliperLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "CustardPlumbingLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "PhoneNumber2",
                table: "CustardEntities",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "CustardCornLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "CustardCaliperLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "CornPlumbingLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnixMatchDate",
                table: "CornCaliperLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SandPlumbingLinks_UnixMatchDate",
                table: "SandPlumbingLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_SandCornLinks_UnixMatchDate",
                table: "SandCornLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_SandCaliperLinks_UnixMatchDate",
                table: "SandCaliperLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Source",
                table: "PlumbingEntities",
                columns: new[] { "PhoneNumber", "Source" });

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingCaliperLinks_UnixMatchDate",
                table: "PlumbingCaliperLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustardPlumbingLinks_UnixMatchDate",
                table: "CustardPlumbingLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustardCornLinks_UnixMatchDate",
                table: "CustardCornLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustardCaliperLinks_UnixMatchDate",
                table: "CustardCaliperLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_CornPlumbingLinks_UnixMatchDate",
                table: "CornPlumbingLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_CornEntities_PhoneNumber_Source",
                table: "CornEntities",
                columns: new[] { "PhoneNumber", "Source" });

            migrationBuilder.CreateIndex(
                name: "IX_CornCaliperLinks_UnixMatchDate",
                table: "CornCaliperLinks",
                column: "UnixMatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_CaliperEntities_Date",
                table: "CaliperEntities",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SandPlumbingLinks_UnixMatchDate",
                table: "SandPlumbingLinks");

            migrationBuilder.DropIndex(
                name: "IX_SandCornLinks_UnixMatchDate",
                table: "SandCornLinks");

            migrationBuilder.DropIndex(
                name: "IX_SandCaliperLinks_UnixMatchDate",
                table: "SandCaliperLinks");

            migrationBuilder.DropIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Source",
                table: "PlumbingEntities");

            migrationBuilder.DropIndex(
                name: "IX_PlumbingCaliperLinks_UnixMatchDate",
                table: "PlumbingCaliperLinks");

            migrationBuilder.DropIndex(
                name: "IX_CustardPlumbingLinks_UnixMatchDate",
                table: "CustardPlumbingLinks");

            migrationBuilder.DropIndex(
                name: "IX_CustardCornLinks_UnixMatchDate",
                table: "CustardCornLinks");

            migrationBuilder.DropIndex(
                name: "IX_CustardCaliperLinks_UnixMatchDate",
                table: "CustardCaliperLinks");

            migrationBuilder.DropIndex(
                name: "IX_CornPlumbingLinks_UnixMatchDate",
                table: "CornPlumbingLinks");

            migrationBuilder.DropIndex(
                name: "IX_CornEntities_PhoneNumber_Source",
                table: "CornEntities");

            migrationBuilder.DropIndex(
                name: "IX_CornCaliperLinks_UnixMatchDate",
                table: "CornCaliperLinks");

            migrationBuilder.DropIndex(
                name: "IX_CaliperEntities_Date",
                table: "CaliperEntities");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "SandPlumbingLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "SandCornLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "SandCaliperLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "PlumbingCaliperLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "CustardPlumbingLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "CustardCornLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "CustardCaliperLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "CornPlumbingLinks");

            migrationBuilder.DropColumn(
                name: "UnixMatchDate",
                table: "CornCaliperLinks");

            migrationBuilder.AlterColumn<long>(
                name: "PhoneNumber2",
                table: "CustardEntities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Source",
                table: "PlumbingEntities",
                columns: new[] { "PhoneNumber", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CornEntities_PhoneNumber_Source",
                table: "CornEntities",
                columns: new[] { "PhoneNumber", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaliperEntities_PhoneNumber_Date",
                table: "CaliperEntities",
                columns: new[] { "PhoneNumber", "Date" },
                unique: true);
        }
    }
}
