using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mafia.Domain.Migrations
{
    public partial class updateUserVote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayCount",
                table: "RoomStagePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayCount",
                table: "RoomStagePlayers");
        }
    }
}
