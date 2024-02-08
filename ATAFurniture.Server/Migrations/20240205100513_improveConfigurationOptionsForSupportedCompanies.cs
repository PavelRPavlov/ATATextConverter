using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATAFurniture.Server.Migrations
{
    /// <inheritdoc />
    public partial class improveConfigurationOptionsForSupportedCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastSelectedCompany_Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSelectedCompany_Email",
                table: "Users");
        }
    }
}
