using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierRefusals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ServiceBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierRefusals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierRefusals_Suppliers_SupplierCode",
                        column: x => x.SupplierCode,
                        principalTable: "Suppliers",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierRefusals_SupplierCode",
                table: "SupplierRefusals",
                column: "SupplierCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierRefusals");
        }
    }
}
