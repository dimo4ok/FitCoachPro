using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCoachPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CoachNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptingNewClients",
                table: "Coaches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptingNewClients",
                table: "Coaches");
        }
    }
}
