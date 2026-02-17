using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadPipe.Infrastructure.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SandPlumbingLinks_SandId",
                table: "SandPlumbingLinks");

            migrationBuilder.DropIndex(
                name: "IX_SandCornLinks_SandId",
                table: "SandCornLinks");

            migrationBuilder.DropIndex(
                name: "IX_SandCaliperLinks_SandId",
                table: "SandCaliperLinks");

            migrationBuilder.DropIndex(
                name: "IX_PlumbingEntities_PhoneNumber",
                table: "PlumbingEntities");

            migrationBuilder.DropIndex(
                name: "IX_PlumbingCaliperLinks_PlumbingId",
                table: "PlumbingCaliperLinks");

            migrationBuilder.DropIndex(
                name: "IX_CustardPlumbingLinks_CustardId",
                table: "CustardPlumbingLinks");

            migrationBuilder.DropIndex(
                name: "IX_CustardCornLinks_CustardId",
                table: "CustardCornLinks");

            migrationBuilder.DropIndex(
                name: "IX_CustardCaliperLinks_CustardId",
                table: "CustardCaliperLinks");

            migrationBuilder.DropIndex(
                name: "IX_CornPlumbingLinks_CornId",
                table: "CornPlumbingLinks");

            migrationBuilder.DropIndex(
                name: "IX_CornEntities_PhoneNumber",
                table: "CornEntities");

            migrationBuilder.DropIndex(
                name: "IX_CornCaliperLinks_CornId",
                table: "CornCaliperLinks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SandPlumbingLinks_SandId",
                table: "SandPlumbingLinks",
                column: "SandId");

            migrationBuilder.CreateIndex(
                name: "IX_SandCornLinks_SandId",
                table: "SandCornLinks",
                column: "SandId");

            migrationBuilder.CreateIndex(
                name: "IX_SandCaliperLinks_SandId",
                table: "SandCaliperLinks",
                column: "SandId");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_PhoneNumber",
                table: "PlumbingEntities",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingCaliperLinks_PlumbingId",
                table: "PlumbingCaliperLinks",
                column: "PlumbingId");

            migrationBuilder.CreateIndex(
                name: "IX_CustardPlumbingLinks_CustardId",
                table: "CustardPlumbingLinks",
                column: "CustardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustardCornLinks_CustardId",
                table: "CustardCornLinks",
                column: "CustardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustardCaliperLinks_CustardId",
                table: "CustardCaliperLinks",
                column: "CustardId");

            migrationBuilder.CreateIndex(
                name: "IX_CornPlumbingLinks_CornId",
                table: "CornPlumbingLinks",
                column: "CornId");

            migrationBuilder.CreateIndex(
                name: "IX_CornEntities_PhoneNumber",
                table: "CornEntities",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CornCaliperLinks_CornId",
                table: "CornCaliperLinks",
                column: "CornId");
        }
    }
}
