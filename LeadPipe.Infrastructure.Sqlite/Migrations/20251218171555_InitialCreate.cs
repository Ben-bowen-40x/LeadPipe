using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadPipe.Infrastructure.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CallEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PhoneNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    CallDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnixCallDate = table.Column<long>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<long>(type: "INTEGER", nullable: false),
                    Billable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlumbingEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PhoneNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnixDate = table.Column<long>(type: "INTEGER", nullable: false),
                    Contents = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    MetaData = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlumbingEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubsEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnixDate = table.Column<long>(type: "INTEGER", nullable: false),
                    SubDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnixSubDate = table.Column<long>(type: "INTEGER", nullable: false),
                    Number = table.Column<long>(type: "INTEGER", nullable: false),
                    Number2 = table.Column<long>(type: "INTEGER", nullable: false),
                    CancelDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnixCancelDate = table.Column<long>(type: "INTEGER", nullable: false),
                    SubCancelDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnixSubCancelDate = table.Column<long>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Complete = table.Column<bool>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Seller = table.Column<string>(type: "TEXT", nullable: true),
                    Seller2 = table.Column<string>(type: "TEXT", nullable: true),
                    Seller3 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubsEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    LastProcessedId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnixLastSyncUtc = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlumbingCallLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlumbingId = table.Column<long>(type: "INTEGER", nullable: false),
                    CallId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlumbingCallLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlumbingCallLinks_CallEntities_CallId",
                        column: x => x.CallId,
                        principalTable: "CallEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlumbingCallLinks_PlumbingEntities_PlumbingId",
                        column: x => x.PlumbingId,
                        principalTable: "PlumbingEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubsCallLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubsId = table.Column<long>(type: "INTEGER", nullable: false),
                    CallId = table.Column<long>(type: "INTEGER", nullable: false),
                    MatchingNumber = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubsCallLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubsCallLinks_CallEntities_CallId",
                        column: x => x.CallId,
                        principalTable: "CallEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubsCallLinks_SubsEntities_SubsId",
                        column: x => x.SubsId,
                        principalTable: "SubsEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubsPlumbingLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubsId = table.Column<long>(type: "INTEGER", nullable: false),
                    PlumbingId = table.Column<long>(type: "INTEGER", nullable: false),
                    MatchingSubPhone = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubsPlumbingLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubsPlumbingLinks_PlumbingEntities_PlumbingId",
                        column: x => x.PlumbingId,
                        principalTable: "PlumbingEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubsPlumbingLinks_SubsEntities_SubsId",
                        column: x => x.SubsId,
                        principalTable: "SubsEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CallEntities_PhoneNumber",
                table: "CallEntities",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingCallLinks_CallId",
                table: "PlumbingCallLinks",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingCallLinks_PlumbingId",
                table: "PlumbingCallLinks",
                column: "PlumbingId");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingCallLinks_PlumbingId_CallId",
                table: "PlumbingCallLinks",
                columns: new[] { "PlumbingId", "CallId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_PhoneNumber",
                table: "PlumbingEntities",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PlumbingEntities_PhoneNumber_Source",
                table: "PlumbingEntities",
                columns: new[] { "PhoneNumber", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubsCallLinks_CallId",
                table: "SubsCallLinks",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_SubsCallLinks_SubsId",
                table: "SubsCallLinks",
                column: "SubsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubsCallLinks_SubsId_CallId",
                table: "SubsCallLinks",
                columns: new[] { "SubsId", "CallId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubsEntities_Number",
                table: "SubsEntities",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_SubsEntities_Number2",
                table: "SubsEntities",
                column: "Number2");

            migrationBuilder.CreateIndex(
                name: "IX_SubsPlumbingLinks_PlumbingId",
                table: "SubsPlumbingLinks",
                column: "PlumbingId");

            migrationBuilder.CreateIndex(
                name: "IX_SubsPlumbingLinks_SubsId",
                table: "SubsPlumbingLinks",
                column: "SubsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubsPlumbingLinks_SubsId_PlumbingId",
                table: "SubsPlumbingLinks",
                columns: new[] { "SubsId", "PlumbingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyncState_Id",
                table: "SyncState",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlumbingCallLinks");

            migrationBuilder.DropTable(
                name: "SubsCallLinks");

            migrationBuilder.DropTable(
                name: "SubsPlumbingLinks");

            migrationBuilder.DropTable(
                name: "SyncState");

            migrationBuilder.DropTable(
                name: "CallEntities");

            migrationBuilder.DropTable(
                name: "PlumbingEntities");

            migrationBuilder.DropTable(
                name: "SubsEntities");
        }
    }
}
