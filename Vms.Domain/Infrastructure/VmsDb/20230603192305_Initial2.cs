using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Company_Id",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Company");

            migrationBuilder.AlterSequence(
                name: "CustomerIds",
                oldIncrementBy: 10);

            migrationBuilder.AlterSequence(
                name: "CompanyIds",
                oldIncrementBy: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Company_Id",
                table: "Company",
                column: "Id");

            migrationBuilder.AlterSequence(
                name: "CustomerIds",
                incrementBy: 10);

            migrationBuilder.AlterSequence(
                name: "CompanyIds",
                incrementBy: 10);
        }
    }
}
