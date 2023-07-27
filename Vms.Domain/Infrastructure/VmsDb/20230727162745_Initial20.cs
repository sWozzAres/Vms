using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceBookingFollowers",
                table: "ServiceBookingFollowers");

            migrationBuilder.RenameTable(
                name: "ServiceBookingFollowers",
                newName: "Followers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followers",
                table: "Followers",
                columns: new[] { "DocumentId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Followers",
                table: "Followers");

            migrationBuilder.RenameTable(
                name: "Followers",
                newName: "ServiceBookingFollowers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceBookingFollowers",
                table: "ServiceBookingFollowers",
                columns: new[] { "DocumentId", "UserId" });
        }
    }
}
