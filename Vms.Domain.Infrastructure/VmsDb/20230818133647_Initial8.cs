using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "ServiceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceEvents_Vehicles_CompanyCode_VehicleId",
                table: "ServiceEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_MotEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvents",
                columns: new[] { "CompanyCode", "VehicleId", "ServiceBookingId" },
                principalTable: "ServiceBookings",
                principalColumns: new[] { "CompanyCode", "VehicleId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "ServiceEvents",
                columns: new[] { "CompanyCode", "VehicleId", "ServiceBookingId" },
                principalTable: "ServiceBookings",
                principalColumns: new[] { "CompanyCode", "VehicleId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceEvents_Vehicles_CompanyCode_VehicleId",
                table: "ServiceEvents",
                columns: new[] { "CompanyCode", "VehicleId" },
                principalTable: "Vehicles",
                principalColumns: new[] { "CompanyCode", "Id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "ServiceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceEvents_Vehicles_CompanyCode_VehicleId",
                table: "ServiceEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_MotEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvents",
                columns: new[] { "CompanyCode", "VehicleId", "ServiceBookingId" },
                principalTable: "ServiceBookings",
                principalColumns: new[] { "CompanyCode", "VehicleId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                table: "ServiceEvents",
                columns: new[] { "CompanyCode", "VehicleId", "ServiceBookingId" },
                principalTable: "ServiceBookings",
                principalColumns: new[] { "CompanyCode", "VehicleId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceEvents_Vehicles_CompanyCode_VehicleId",
                table: "ServiceEvents",
                columns: new[] { "CompanyCode", "VehicleId" },
                principalTable: "Vehicles",
                principalColumns: new[] { "CompanyCode", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
