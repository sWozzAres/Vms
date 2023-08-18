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
            migrationBuilder.DropColumn(
                name: "MotDue",
                table: "ServiceBookings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ServiceBookingLocks",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookingLocks_UserId",
                table: "ServiceBookingLocks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBookingLocks_Users_UserId",
                table: "ServiceBookingLocks",
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
                name: "FK_ServiceBookingLocks_Users_UserId",
                table: "ServiceBookingLocks");

            migrationBuilder.DropIndex(
                name: "IX_ServiceBookingLocks_UserId",
                table: "ServiceBookingLocks");

            migrationBuilder.AddColumn<DateOnly>(
                name: "MotDue",
                table: "ServiceBookings",
                type: "date",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ServiceBookingLocks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");
        }
    }
}
