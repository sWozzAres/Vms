using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotEvents_Vehicles_CompanyCode_VehicleId",
                table: "MotEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_MotEvents_Vehicles_CompanyCode_VehicleId",
                table: "MotEvents",
                columns: new[] { "CompanyCode", "VehicleId" },
                principalTable: "Vehicles",
                principalColumns: new[] { "CompanyCode", "Id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MotEvents_Vehicles_CompanyCode_VehicleId",
                table: "MotEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_MotEvents_Vehicles_CompanyCode_VehicleId",
                table: "MotEvents",
                columns: new[] { "CompanyCode", "VehicleId" },
                principalTable: "Vehicles",
                principalColumns: new[] { "CompanyCode", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
