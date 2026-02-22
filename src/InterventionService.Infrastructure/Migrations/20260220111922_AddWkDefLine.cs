using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterventionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWkDefLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinutes",
                table: "work_definitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "work_definitions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkDefinitionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnitPriceExclTax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    VatRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    WorkDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkDefinitionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkDefinitionLines_work_definitions_WorkDefinitionId",
                        column: x => x.WorkDefinitionId,
                        principalTable: "work_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkDefinitionLines_ProductId",
                table: "WorkDefinitionLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDefinitionLines_WorkDefinitionId",
                table: "WorkDefinitionLines",
                column: "WorkDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkDefinitionLines");

            migrationBuilder.DropColumn(
                name: "EstimatedMinutes",
                table: "work_definitions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "work_definitions");
        }
    }
}
