using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotEvent_ServiceBooking_CompanyCode_ServiceBookingId",
                table: "MotEvent");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ServiceBooking_CompanyCode_Id",
                table: "ServiceBooking");

            migrationBuilder.DropIndex(
                name: "IX_ServiceBooking_CompanyCode_VehicleId",
                table: "ServiceBooking");

            migrationBuilder.DropIndex(
                name: "IX_MotEvent_CompanyCode_ServiceBookingId",
                table: "MotEvent");

            migrationBuilder.DropIndex(
                name: "IX_MotEvent_CompanyCode_VehicleId",
                table: "MotEvent");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ServiceBooking_CompanyCode_VehicleId_Id",
                table: "ServiceBooking",
                columns: new[] { "CompanyCode", "VehicleId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_MotEvent_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvent",
                columns: new[] { "CompanyCode", "VehicleId", "ServiceBookingId" },
                unique: true,
                filter: "[ServiceBookingId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MotEvent_ServiceBooking_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvent",
                columns: new[] { "CompanyCode", "VehicleId", "ServiceBookingId" },
                principalTable: "ServiceBooking",
                principalColumns: new[] { "CompanyCode", "VehicleId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotEvent_ServiceBooking_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvent");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ServiceBooking_CompanyCode_VehicleId_Id",
                table: "ServiceBooking");

            migrationBuilder.DropIndex(
                name: "IX_MotEvent_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvent");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ServiceBooking_CompanyCode_Id",
                table: "ServiceBooking",
                columns: new[] { "CompanyCode", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBooking_CompanyCode_VehicleId",
                table: "ServiceBooking",
                columns: new[] { "CompanyCode", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_MotEvent_CompanyCode_ServiceBookingId",
                table: "MotEvent",
                columns: new[] { "CompanyCode", "ServiceBookingId" },
                unique: true,
                filter: "[ServiceBookingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MotEvent_CompanyCode_VehicleId",
                table: "MotEvent",
                columns: new[] { "CompanyCode", "VehicleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MotEvent_ServiceBooking_CompanyCode_ServiceBookingId",
                table: "MotEvent",
                columns: new[] { "CompanyCode", "ServiceBookingId" },
                principalTable: "ServiceBooking",
                principalColumns: new[] { "CompanyCode", "Id" });
        }
    }
}
