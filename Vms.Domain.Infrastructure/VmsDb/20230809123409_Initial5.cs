using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityTags_Companies_CompanyCode",
                schema: "System",
                table: "EntityTags");

            migrationBuilder.DropIndex(
                name: "IX_EntityTags_CompanyCode_Id_EntityKind",
                schema: "System",
                table: "EntityTags");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyCode",
                schema: "System",
                table: "EntityTags",
                type: "nchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_EntityTags_CompanyCode_Id_EntityKind",
                schema: "System",
                table: "EntityTags",
                columns: new[] { "CompanyCode", "Id", "EntityKind" },
                unique: true,
                filter: "[CompanyCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTags_EntityKey_EntityKind",
                schema: "System",
                table: "EntityTags",
                columns: new[] { "EntityKey", "EntityKind" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityTags_Companies_CompanyCode",
                schema: "System",
                table: "EntityTags",
                column: "CompanyCode",
                principalTable: "Companies",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityTags_Companies_CompanyCode",
                schema: "System",
                table: "EntityTags");

            migrationBuilder.DropIndex(
                name: "IX_EntityTags_CompanyCode_Id_EntityKind",
                schema: "System",
                table: "EntityTags");

            migrationBuilder.DropIndex(
                name: "IX_EntityTags_EntityKey_EntityKind",
                schema: "System",
                table: "EntityTags");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyCode",
                schema: "System",
                table: "EntityTags",
                type: "nchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityTags_CompanyCode_Id_EntityKind",
                schema: "System",
                table: "EntityTags",
                columns: new[] { "CompanyCode", "Id", "EntityKind" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityTags_Companies_CompanyCode",
                schema: "System",
                table: "EntityTags",
                column: "CompanyCode",
                principalTable: "Companies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
