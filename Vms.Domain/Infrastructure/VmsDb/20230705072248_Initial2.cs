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
            migrationBuilder.CreateTable(
                name: "RefusalReasons",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefusalReasons", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_RefusalReason_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "RescheduleReasons",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RescheduleReasons", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_RescheduleReason_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefusalReasons");

            migrationBuilder.DropTable(
                name: "RescheduleReasons");
        }
    }
}
