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
            migrationBuilder.CreateTable(
                name: "ConfirmBookedRefusalReasons",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmBookedRefusalReasons", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_ConfirmBookedRefusalReason_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmBookedRefusalReasons");
        }
    }
}
