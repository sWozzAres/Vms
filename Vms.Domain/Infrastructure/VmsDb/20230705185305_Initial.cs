using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address_Street = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address_Locality = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address_Town = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address_Postcode = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Address_Location = table.Column<Geometry>(type: "geography", nullable: false),
                    IsIndependent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "VehicleMake",
                columns: table => new
                {
                    Make = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Make", x => x.Make);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Customer_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Driver",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Salutation = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    FirstName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    MiddleNames = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    LastName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    EmailAddress = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    MobileNumber = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    HomeLocation = table.Column<Geometry>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.Id);
                    table.UniqueConstraint("AK_Driver_CompanyCode_EmailAddress", x => new { x.CompanyCode, x.EmailAddress });
                    table.UniqueConstraint("AK_Driver_CompanyCode_Id", x => new { x.CompanyCode, x.Id });
                    table.ForeignKey(
                        name: "FK_Driver_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Fleet",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fleet", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Fleet_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Network",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Network", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Network_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                });

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

            migrationBuilder.CreateTable(
                name: "SupplierFranchise",
                columns: table => new
                {
                    SupplierCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    Franchise = table.Column<string>(type: "varchar(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierFranchise", x => new { x.SupplierCode, x.Franchise });
                    table.ForeignKey(
                        name: "FK_SupplierFranchise_Supplier_SupplierCode",
                        column: x => x.SupplierCode,
                        principalTable: "Supplier",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierFranchise_VehicleMake_Franchise",
                        column: x => x.Franchise,
                        principalTable: "VehicleMake",
                        principalColumn: "Make",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModel",
                columns: table => new
                {
                    Make = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Model = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Model", x => new { x.Make, x.Model });
                    table.ForeignKey(
                        name: "FK_VehicleModel_VehicleMake_Make",
                        column: x => x.Make,
                        principalTable: "VehicleMake",
                        principalColumn: "Make",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerNetwork",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    NetworkCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNetwork", x => new { x.CompanyCode, x.NetworkCode, x.CustomerCode });
                    table.ForeignKey(
                        name: "FK_CustomerNetwork_Customer",
                        columns: x => new { x.CompanyCode, x.CustomerCode },
                        principalTable: "Customer",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_CustomerNetwork_Network",
                        columns: x => new { x.CompanyCode, x.NetworkCode },
                        principalTable: "Network",
                        principalColumns: new[] { "CompanyCode", "Code" });
                });

            migrationBuilder.CreateTable(
                name: "FleetNetwork",
                columns: table => new
                {
                    FleetCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    NetworkCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetNetwork", x => new { x.CompanyCode, x.FleetCode, x.NetworkCode });
                    table.ForeignKey(
                        name: "FK_FleetNetwork_Fleet",
                        columns: x => new { x.CompanyCode, x.FleetCode },
                        principalTable: "Fleet",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_FleetNetwork_Network",
                        columns: x => new { x.CompanyCode, x.NetworkCode },
                        principalTable: "Network",
                        principalColumns: new[] { "CompanyCode", "Code" });
                });

            migrationBuilder.CreateTable(
                name: "NetworkSupplier",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", nullable: false),
                    NetworkCode = table.Column<string>(type: "nchar(10)", nullable: false),
                    SupplierCode = table.Column<string>(type: "varchar(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkSupplier", x => new { x.CompanyCode, x.NetworkCode, x.SupplierCode });
                    table.ForeignKey(
                        name: "FK_NetworkSupplier_Network",
                        columns: x => new { x.CompanyCode, x.NetworkCode },
                        principalTable: "Network",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_NetworkSupplier_Supplier",
                        column: x => x.SupplierCode,
                        principalTable: "Supplier",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Make = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Model = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ChassisNumber = table.Column<string>(type: "varchar(18)", unicode: false, maxLength: 18, nullable: true),
                    DateFirstRegistered = table.Column<DateOnly>(type: "date", nullable: false),
                    Address_Street = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address_Locality = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address_Town = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address_Postcode = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Address_Location = table.Column<Geometry>(type: "geography", nullable: false),
                    CustomerCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    FleetCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.UniqueConstraint("AK_Vehicle_CompanyCode_Id", x => new { x.CompanyCode, x.Id });
                    table.ForeignKey(
                        name: "FK_Vehicle_Company",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_Vehicle_Customer",
                        columns: x => new { x.CompanyCode, x.CustomerCode },
                        principalTable: "Customer",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_Vehicle_Fleet",
                        columns: x => new { x.CompanyCode, x.FleetCode },
                        principalTable: "Fleet",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_Vehicle_VehicleModel",
                        columns: x => new { x.Make, x.Model },
                        principalTable: "VehicleModel",
                        principalColumns: new[] { "Make", "Model" });
                });

            migrationBuilder.CreateTable(
                name: "DriverVehicles",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverVehicles", x => new { x.CompanyCode, x.DriverId, x.VehicleId });
                    table.ForeignKey(
                        name: "FK_DriverVehicles_Driver",
                        columns: x => new { x.CompanyCode, x.DriverId },
                        principalTable: "Driver",
                        principalColumns: new[] { "CompanyCode", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverVehicles_Vehicle",
                        columns: x => new { x.CompanyCode, x.VehicleId },
                        principalTable: "Vehicle",
                        principalColumns: new[] { "CompanyCode", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "ServiceBooking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreferredDate1 = table.Column<DateOnly>(type: "date", nullable: true),
                    PreferredDate2 = table.Column<DateOnly>(type: "date", nullable: true),
                    PreferredDate3 = table.Column<DateOnly>(type: "date", nullable: true),
                    MotDue = table.Column<DateOnly>(type: "date", nullable: true),
                    SupplierCode = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true),
                    BookedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBooking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceBooking_Supplier_SupplierCode",
                        column: x => x.SupplierCode,
                        principalTable: "Supplier",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_ServiceBookings_Vehicle",
                        columns: x => new { x.CompanyCode, x.VehicleId },
                        principalTable: "Vehicle",
                        principalColumns: new[] { "CompanyCode", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleMot",
                columns: table => new
                {
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Due = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleMot", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicle_VehicleMot",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleVrm",
                columns: table => new
                {
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    Vrm = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleVrm", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicle_VehicleVrm",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNetwork_CompanyCode_CustomerCode",
                table: "CustomerNetwork",
                columns: new[] { "CompanyCode", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverVehicles_CompanyCode_VehicleId",
                table: "DriverVehicles",
                columns: new[] { "CompanyCode", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_FleetNetwork_CompanyCode_NetworkCode",
                table: "FleetNetwork",
                columns: new[] { "CompanyCode", "NetworkCode" });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkSupplier_SupplierCode",
                table: "NetworkSupplier",
                column: "SupplierCode");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBooking_CompanyCode_VehicleId",
                table: "ServiceBooking",
                columns: new[] { "CompanyCode", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBooking_SupplierCode",
                table: "ServiceBooking",
                column: "SupplierCode");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierFranchise_Franchise",
                table: "SupplierFranchise",
                column: "Franchise");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CompanyCode_CustomerCode",
                table: "Vehicle",
                columns: new[] { "CompanyCode", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CompanyCode_FleetCode",
                table: "Vehicle",
                columns: new[] { "CompanyCode", "FleetCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Make_Model",
                table: "Vehicle",
                columns: new[] { "Make", "Model" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerNetwork");

            migrationBuilder.DropTable(
                name: "DriverVehicles");

            migrationBuilder.DropTable(
                name: "FleetNetwork");

            migrationBuilder.DropTable(
                name: "NetworkSupplier");

            migrationBuilder.DropTable(
                name: "RefusalReasons");

            migrationBuilder.DropTable(
                name: "RescheduleReasons");

            migrationBuilder.DropTable(
                name: "ServiceBooking");

            migrationBuilder.DropTable(
                name: "SupplierFranchise");

            migrationBuilder.DropTable(
                name: "VehicleMot");

            migrationBuilder.DropTable(
                name: "VehicleVrm")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.DropTable(
                name: "Driver");

            migrationBuilder.DropTable(
                name: "Network");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Fleet");

            migrationBuilder.DropTable(
                name: "VehicleModel");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "VehicleMake");
        }
    }
}
