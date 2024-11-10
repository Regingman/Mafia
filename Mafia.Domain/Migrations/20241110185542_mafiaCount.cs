using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mafia.Domain.Migrations
{
    public partial class mafiaCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MafiaCount",
                table: "RoomStagePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MafiaCount",
                table: "RoomStagePlayers");
        }
    }
}
