using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleLocation",
                table: "ServiceBooking");

            migrationBuilder.RenameColumn(
                name: "HomeLocation",
                table: "Vehicle",
                newName: "Address_Location");

            migrationBuilder.AddColumn<string>(
                name: "Address_Locality",
                table: "Vehicle",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Postcode",
                table: "Vehicle",
                type: "varchar(8)",
                unicode: false,
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Vehicle",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Town",
                table: "Vehicle",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Address_Town",
                table: "Supplier",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address_Street",
                table: "Supplier",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address_Postcode",
                table: "Supplier",
                type: "varchar(8)",
                unicode: false,
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(9)",
                oldUnicode: false,
                oldMaxLength: 9);

            migrationBuilder.AlterColumn<string>(
                name: "Address_Locality",
                table: "Supplier",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Locality",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Address_Postcode",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Address_Town",
                table: "Vehicle");

            migrationBuilder.RenameColumn(
                name: "Address_Location",
                table: "Vehicle",
                newName: "HomeLocation");

            migrationBuilder.AlterColumn<string>(
                name: "Address_Town",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Address_Street",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Address_Postcode",
                table: "Supplier",
                type: "varchar(9)",
                unicode: false,
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldUnicode: false,
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "Address_Locality",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Point>(
                name: "VehicleLocation",
                table: "ServiceBooking",
                type: "geography",
                nullable: false);
        }
    }
}
