using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBookingSupplier_Supplier_SupplierCode1",
                table: "ServiceBookingSupplier");

            migrationBuilder.DropIndex(
                name: "IX_ServiceBookingSupplier_SupplierCode1",
                table: "ServiceBookingSupplier");

            migrationBuilder.DropColumn(
                name: "SupplierCode1",
                table: "ServiceBookingSupplier")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingSupplierHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierCode",
                table: "ServiceBookingSupplier",
                type: "varchar(8)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingSupplierHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "ServiceBookingSupplierHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookingSupplier_SupplierCode",
                table: "ServiceBookingSupplier",
                column: "SupplierCode");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBookingSupplier_Supplier_SupplierCode",
                table: "ServiceBookingSupplier",
                column: "SupplierCode",
                principalTable: "Supplier",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBookingSupplier_Supplier_SupplierCode",
                table: "ServiceBookingSupplier");

            migrationBuilder.DropIndex(
                name: "IX_ServiceBookingSupplier_SupplierCode",
                table: "ServiceBookingSupplier");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierCode",
                table: "ServiceBookingSupplier",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(8)")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingSupplierHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "ServiceBookingSupplierHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<string>(
                name: "SupplierCode1",
                table: "ServiceBookingSupplier",
                type: "varchar(8)",
                nullable: false,
                defaultValue: "")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingSupplierHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookingSupplier_SupplierCode1",
                table: "ServiceBookingSupplier",
                column: "SupplierCode1");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBookingSupplier_Supplier_SupplierCode1",
                table: "ServiceBookingSupplier",
                column: "SupplierCode1",
                principalTable: "Supplier",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
