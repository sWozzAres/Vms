using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scrum.Api.Domain.Infrastructure.ScrumDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductIncrements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SprintGoal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIncrements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductBacklogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    EstimatedDays = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFixedDeliveryDate = table.Column<bool>(type: "bit", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: true),
                    Roi = table.Column<int>(type: "int", nullable: true),
                    ProductIncrementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBacklogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBacklogItems_ProductIncrements_ProductIncrementId",
                        column: x => x.ProductIncrementId,
                        principalTable: "ProductIncrements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductBacklogItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintBacklogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductBacklogItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DependsOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintBacklogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintBacklogItems_ProductBacklogItems_ProductBacklogItemId",
                        column: x => x.ProductBacklogItemId,
                        principalTable: "ProductBacklogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SprintBacklogItems_SprintBacklogItems_DependsOnId",
                        column: x => x.DependsOnId,
                        principalTable: "SprintBacklogItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_ProductId",
                table: "ProductBacklogItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_ProductIncrementId",
                table: "ProductBacklogItems",
                column: "ProductIncrementId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintBacklogItems_DependsOnId",
                table: "SprintBacklogItems",
                column: "DependsOnId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintBacklogItems_ProductBacklogItemId",
                table: "SprintBacklogItems",
                column: "ProductBacklogItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SprintBacklogItems");

            migrationBuilder.DropTable(
                name: "ProductBacklogItems");

            migrationBuilder.DropTable(
                name: "ProductIncrements");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
