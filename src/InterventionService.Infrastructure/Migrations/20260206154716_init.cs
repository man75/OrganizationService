using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterventionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    DefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TechnicianId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_order_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    unit_price_excl_tax_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    unit_price_excl_tax_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    VatRate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_work_order_lines_work_orders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "work_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_work_definitions_OrganizationId_Name",
                table: "work_definitions",
                columns: new[] { "OrganizationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_definitions_OrganizationId_Status",
                table: "work_definitions",
                columns: new[] { "OrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_work_order_lines_WorkOrderId",
                table: "work_order_lines",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_OrganizationId_ClientId",
                table: "work_orders",
                columns: new[] { "OrganizationId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_OrganizationId_DefinitionId",
                table: "work_orders",
                columns: new[] { "OrganizationId", "DefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_OrganizationId_ScheduledAt",
                table: "work_orders",
                columns: new[] { "OrganizationId", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_OrganizationId_Status",
                table: "work_orders",
                columns: new[] { "OrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_OrganizationId_VehicleId",
                table: "work_orders",
                columns: new[] { "OrganizationId", "VehicleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_definitions");

            migrationBuilder.DropTable(
                name: "work_order_lines");

            migrationBuilder.DropTable(
                name: "work_orders");
        }
    }
}
