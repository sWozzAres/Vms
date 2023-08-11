using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Followers",
                schema: "System",
                table: "Followers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "System",
                table: "TaskLogs",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "System",
                table: "Followers",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                schema: "System",
                table: "Followers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "System",
                table: "ActivityLog",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followers",
                schema: "System",
                table: "Followers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RecentViews",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ViewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecentViews_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "System",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskLogs_UserId",
                schema: "System",
                table: "TaskLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Followers_DocumentId_UserId",
                schema: "System",
                table: "Followers",
                columns: new[] { "DocumentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Followers_UserId",
                schema: "System",
                table: "Followers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_UserId",
                schema: "System",
                table: "ActivityLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentViews_UserId",
                schema: "System",
                table: "RecentViews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_Users_UserId",
                schema: "System",
                table: "ActivityLog",
                column: "UserId",
                principalSchema: "System",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_Users_UserId",
                schema: "System",
                table: "Followers",
                column: "UserId",
                principalSchema: "System",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLogs_Users_UserId",
                schema: "System",
                table: "TaskLogs",
                column: "UserId",
                principalSchema: "System",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_Users_UserId",
                schema: "System",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_Followers_Users_UserId",
                schema: "System",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskLogs_Users_UserId",
                schema: "System",
                table: "TaskLogs");

            migrationBuilder.DropTable(
                name: "RecentViews",
                schema: "System");

            migrationBuilder.DropIndex(
                name: "IX_TaskLogs_UserId",
                schema: "System",
                table: "TaskLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Followers",
                schema: "System",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_DocumentId_UserId",
                schema: "System",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_UserId",
                schema: "System",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_UserId",
                schema: "System",
                table: "ActivityLog");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "System",
                table: "TaskLogs");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "System",
                table: "Followers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "System",
                table: "Followers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "System",
                table: "ActivityLog",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followers",
                schema: "System",
                table: "Followers",
                columns: new[] { "DocumentId", "UserId" });
        }
    }
}
