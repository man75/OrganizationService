using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganizationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "siret",
                table: "organizations",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "siret",
                table: "organizations");
        }
    }
}
