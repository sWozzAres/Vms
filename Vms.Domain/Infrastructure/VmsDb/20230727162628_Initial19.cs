using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBookingFollowers_ServiceBooking_ServiceBookingId",
                table: "ServiceBookingFollowers");

            migrationBuilder.RenameColumn(
                name: "ServiceBookingId",
                table: "ServiceBookingFollowers",
                newName: "DocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "ServiceBookingFollowers",
                newName: "ServiceBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBookingFollowers_ServiceBooking_ServiceBookingId",
                table: "ServiceBookingFollowers",
                column: "ServiceBookingId",
                principalTable: "ServiceBooking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
