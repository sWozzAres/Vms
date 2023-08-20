using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Api.Domain.Infrastructure.CatalogDb
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityNotifications",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentKind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DocumentKey = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Read = table.Column<bool>(type: "bit", nullable: false),
                    ActivityLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityNotifications_ActivityLog_ActivityLogId",
                        column: x => x.ActivityLogId,
                        principalSchema: "System",
                        principalTable: "ActivityLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "System",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityNotifications_ActivityLogId",
                schema: "System",
                table: "ActivityNotifications",
                column: "ActivityLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityNotifications_UserId_EntryDate",
                schema: "System",
                table: "ActivityNotifications",
                columns: new[] { "UserId", "EntryDate" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityNotifications",
                schema: "System");
        }
    }
}
