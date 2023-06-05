using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazingFlame.UniversalBanList.Migrations
{
    public partial class TimestampAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Steam64",
                table: "uBanList_universal_ban_whitelists",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Steam64",
                table: "uBanList_universal_ban_whitelists");
        }
    }
}
