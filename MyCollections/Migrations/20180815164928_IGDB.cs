using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCollections.Migrations
{
    public partial class IGDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IGDBId",
                table: "Game",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IGDBId",
                table: "Game");
        }
    }
}
