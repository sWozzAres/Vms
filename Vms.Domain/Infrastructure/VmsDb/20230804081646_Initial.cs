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
            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.EnsureSchema(
                name: "ClientApp");

            migrationBuilder.CreateTable(
                name: "ActivityLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Recipients = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => new { x.DocumentId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
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
                    table.PrimaryKey("PK_Suppliers", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TaskLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Log = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLogs", x => x.Id);
                    table.CheckConstraint("Log record should be formatted as JSON", "ISJSON(log)=1");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "ClientApp",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "VehicleMakes",
                columns: table => new
                {
                    Make = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Make", x => x.Make);
                });

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
                        name: "FK_ConfirmBookedRefusalReasons_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Customers_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
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
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.UniqueConstraint("AK_Drivers_CompanyCode_EmailAddress", x => new { x.CompanyCode, x.EmailAddress });
                    table.UniqueConstraint("AK_Drivers_CompanyCode_Id", x => new { x.CompanyCode, x.Id });
                    table.ForeignKey(
                        name: "FK_Drivers_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Fleets",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fleets", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Fleets_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_Networks_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "NonArrivalReasons",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonArrivalReasons", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_NonArrivalReasons_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "NotCompleteReasons",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotCompleteReasons", x => new { x.CompanyCode, x.Code });
                    table.ForeignKey(
                        name: "FK_NotCompleteReasons_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
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
                        name: "FK_RefusalReasons_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
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
                        name: "FK_RescheduleReasons_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                schema: "ClientApp",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ClientApp",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierFranchises",
                columns: table => new
                {
                    SupplierCode = table.Column<string>(type: "varchar(8)", nullable: false),
                    Franchise = table.Column<string>(type: "varchar(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierFranchises", x => new { x.SupplierCode, x.Franchise });
                    table.ForeignKey(
                        name: "FK_SupplierFranchises_Suppliers_SupplierCode",
                        column: x => x.SupplierCode,
                        principalTable: "Suppliers",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierFranchises_VehicleMakes_Franchise",
                        column: x => x.Franchise,
                        principalTable: "VehicleMakes",
                        principalColumn: "Make",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModels",
                columns: table => new
                {
                    Make = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Model = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Model", x => new { x.Make, x.Model });
                    table.ForeignKey(
                        name: "FK_VehicleModels_VehicleMakes_Make",
                        column: x => x.Make,
                        principalTable: "VehicleMakes",
                        principalColumn: "Make",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerNetworks",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    NetworkCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNetworks", x => new { x.CompanyCode, x.NetworkCode, x.CustomerCode });
                    table.ForeignKey(
                        name: "FK_CustomerNetworks_Customers",
                        columns: x => new { x.CompanyCode, x.CustomerCode },
                        principalTable: "Customers",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_CustomerNetworks_Networks",
                        columns: x => new { x.CompanyCode, x.NetworkCode },
                        principalTable: "Networks",
                        principalColumns: new[] { "CompanyCode", "Code" });
                });

            migrationBuilder.CreateTable(
                name: "FleetNetworks",
                columns: table => new
                {
                    FleetCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    NetworkCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetNetworks", x => new { x.CompanyCode, x.FleetCode, x.NetworkCode });
                    table.ForeignKey(
                        name: "FK_FleetNetworks_Fleets",
                        columns: x => new { x.CompanyCode, x.FleetCode },
                        principalTable: "Fleets",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_FleetNetworks_Networks",
                        columns: x => new { x.CompanyCode, x.NetworkCode },
                        principalTable: "Networks",
                        principalColumns: new[] { "CompanyCode", "Code" });
                });

            migrationBuilder.CreateTable(
                name: "NetworkSuppliers",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nchar(10)", nullable: false),
                    NetworkCode = table.Column<string>(type: "nchar(10)", nullable: false),
                    SupplierCode = table.Column<string>(type: "varchar(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkSuppliers", x => new { x.CompanyCode, x.NetworkCode, x.SupplierCode });
                    table.ForeignKey(
                        name: "FK_NetworkSuppliers_Networks",
                        columns: x => new { x.CompanyCode, x.NetworkCode },
                        principalTable: "Networks",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_NetworkSuppliers_Suppliers",
                        column: x => x.SupplierCode,
                        principalTable: "Suppliers",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
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
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.UniqueConstraint("AK_Vehicles_CompanyCode_Id", x => new { x.CompanyCode, x.Id });
                    table.ForeignKey(
                        name: "FK_Vehicles_Companies",
                        column: x => x.CompanyCode,
                        principalTable: "Companies",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_Vehicles_Customers",
                        columns: x => new { x.CompanyCode, x.CustomerCode },
                        principalTable: "Customers",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_Vehicles_Fleets",
                        columns: x => new { x.CompanyCode, x.FleetCode },
                        principalTable: "Fleets",
                        principalColumns: new[] { "CompanyCode", "Code" });
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleModels",
                        columns: x => new { x.Make, x.Model },
                        principalTable: "VehicleModels",
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
                        name: "FK_DriverVehicles_Drivers",
                        columns: x => new { x.CompanyCode, x.DriverId },
                        principalTable: "Drivers",
                        principalColumns: new[] { "CompanyCode", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverVehicles_Vehicles",
                        columns: x => new { x.CompanyCode, x.VehicleId },
                        principalTable: "Vehicles",
                        principalColumns: new[] { "CompanyCode", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "ServiceBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Ref = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreferredDate1 = table.Column<DateOnly>(type: "date", nullable: true),
                    PreferredDate2 = table.Column<DateOnly>(type: "date", nullable: true),
                    PreferredDate3 = table.Column<DateOnly>(type: "date", nullable: true),
                    MotDue = table.Column<DateOnly>(type: "date", nullable: true),
                    SupplierCode = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true),
                    BookedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EstimatedCompletion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RescheduleTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ServiceLevel = table.Column<int>(type: "int", nullable: false),
                    MotEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OwnerUserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBookings", x => x.Id);
                    table.UniqueConstraint("AK_ServiceBookings_CompanyCode_VehicleId_Id", x => new { x.CompanyCode, x.VehicleId, x.Id });
                    table.ForeignKey(
                        name: "FK_ServiceBookings_Suppliers_SupplierCode",
                        column: x => x.SupplierCode,
                        principalTable: "Suppliers",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_ServiceBookings_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalSchema: "ClientApp",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceBookings_Users_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalSchema: "ClientApp",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceBookings_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "ClientApp",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceBookings_Vehicles",
                        columns: x => new { x.CompanyCode, x.VehicleId },
                        principalTable: "Vehicles",
                        principalColumns: new[] { "CompanyCode", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleMots",
                columns: table => new
                {
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Due = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleMots", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleMots",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleVrms",
                columns: table => new
                {
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    Vrm = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleVrms", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleVrms",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.CreateTable(
                name: "MotEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ServiceBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    CompanyCode = table.Column<string>(type: "nchar(10)", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    Due = table.Column<DateOnly>(type: "date", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotEvents_ServiceBookings_CompanyCode_VehicleId_ServiceBookingId",
                        columns: x => new { x.CompanyCode, x.VehicleId, x.ServiceBookingId },
                        principalTable: "ServiceBookings",
                        principalColumns: new[] { "CompanyCode", "VehicleId", "Id" });
                    table.ForeignKey(
                        name: "FK_MotEvents_Vehicles_CompanyCode_VehicleId",
                        columns: x => new { x.CompanyCode, x.VehicleId },
                        principalTable: "Vehicles",
                        principalColumns: new[] { "CompanyCode", "Id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.CreateTable(
                name: "ServiceBookingContacts",
                columns: table => new
                {
                    ServiceBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    Name = table.Column<string>(type: "nvarchar(41)", maxLength: 41, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    EmailAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    MobileNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBookingContacts", x => x.ServiceBookingId);
                    table.ForeignKey(
                        name: "FK_ServiceBookings_ServiceBookingContacts",
                        column: x => x.ServiceBookingId,
                        principalTable: "ServiceBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.CreateTable(
                name: "ServiceBookingDrivers",
                columns: table => new
                {
                    ServiceBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    Name = table.Column<string>(type: "nvarchar(41)", maxLength: 41, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    EmailAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    MobileNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom"),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBookingDrivers", x => x.ServiceBookingId);
                    table.ForeignKey(
                        name: "FK_ServiceBookings_ServiceBookingDrivers",
                        column: x => x.ServiceBookingId,
                        principalTable: "ServiceBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.CreateTable(
                name: "ServiceBookingLocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Granted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBookingLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceBookingLocks_ServiceBookings_ServiceBookingId",
                        column: x => x.ServiceBookingId,
                        principalTable: "ServiceBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_DocumentId",
                table: "ActivityLog",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNetworks_CompanyCode_CustomerCode",
                table: "CustomerNetworks",
                columns: new[] { "CompanyCode", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverVehicles_CompanyCode_VehicleId",
                table: "DriverVehicles",
                columns: new[] { "CompanyCode", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_FleetNetworks_CompanyCode_NetworkCode",
                table: "FleetNetworks",
                columns: new[] { "CompanyCode", "NetworkCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Logins_UserId",
                schema: "ClientApp",
                table: "Logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MotEvents_CompanyCode_VehicleId_ServiceBookingId",
                table: "MotEvents",
                columns: new[] { "CompanyCode", "VehicleId", "ServiceBookingId" },
                unique: true,
                filter: "[ServiceBookingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MotEvents_VehicleId_IsCurrent",
                table: "MotEvents",
                columns: new[] { "VehicleId", "IsCurrent" },
                unique: true,
                filter: "IsCurrent = 1");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkSuppliers_SupplierCode",
                table: "NetworkSuppliers",
                column: "SupplierCode");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookingLocks_ServiceBookingId",
                table: "ServiceBookingLocks",
                column: "ServiceBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookings_AssignedToUserId",
                table: "ServiceBookings",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookings_CreatedUserId",
                table: "ServiceBookings",
                column: "CreatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookings_OwnerUserId",
                table: "ServiceBookings",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBookings_SupplierCode",
                table: "ServiceBookings",
                column: "SupplierCode");

            migrationBuilder.CreateIndex(
                name: "UQ_ServiceBooking_Ref",
                table: "ServiceBookings",
                column: "Ref",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierFranchises_Franchise",
                table: "SupplierFranchises",
                column: "Franchise");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CompanyCode_CustomerCode",
                table: "Vehicles",
                columns: new[] { "CompanyCode", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CompanyCode_FleetCode",
                table: "Vehicles",
                columns: new[] { "CompanyCode", "FleetCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Make_Model",
                table: "Vehicles",
                columns: new[] { "Make", "Model" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLog");

            migrationBuilder.DropTable(
                name: "ConfirmBookedRefusalReasons");

            migrationBuilder.DropTable(
                name: "CustomerNetworks");

            migrationBuilder.DropTable(
                name: "DriverVehicles");

            migrationBuilder.DropTable(
                name: "Emails",
                schema: "System");

            migrationBuilder.DropTable(
                name: "FleetNetworks");

            migrationBuilder.DropTable(
                name: "Followers");

            migrationBuilder.DropTable(
                name: "Logins",
                schema: "ClientApp");

            migrationBuilder.DropTable(
                name: "MotEvents")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "MotEventsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.DropTable(
                name: "NetworkSuppliers");

            migrationBuilder.DropTable(
                name: "NonArrivalReasons");

            migrationBuilder.DropTable(
                name: "NotCompleteReasons");

            migrationBuilder.DropTable(
                name: "RefusalReasons");

            migrationBuilder.DropTable(
                name: "RescheduleReasons");

            migrationBuilder.DropTable(
                name: "ServiceBookingContacts")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingContactsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.DropTable(
                name: "ServiceBookingDrivers")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBookingDriversHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.DropTable(
                name: "ServiceBookingLocks");

            migrationBuilder.DropTable(
                name: "SupplierFranchises");

            migrationBuilder.DropTable(
                name: "TaskLogs");

            migrationBuilder.DropTable(
                name: "VehicleMots");

            migrationBuilder.DropTable(
                name: "VehicleVrms")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "VehicleVrmsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "ServiceBookings");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "ClientApp");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Fleets");

            migrationBuilder.DropTable(
                name: "VehicleModels");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "VehicleMakes");
        }
    }
}
