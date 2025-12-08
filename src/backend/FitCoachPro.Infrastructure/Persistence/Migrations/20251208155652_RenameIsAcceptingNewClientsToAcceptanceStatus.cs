using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCoachPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsAcceptingNewClientsToAcceptanceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptingNewClients",
                table: "Coaches");

            migrationBuilder.AddColumn<int>(
                name: "AcceptanceStatus",
                table: "Coaches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceStatus",
                table: "Coaches");

            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptingNewClients",
                table: "Coaches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
