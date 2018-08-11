using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCollections.Migrations
{
    public partial class StoreSystemLogos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "System",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Store",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendlyName",
                table: "Game",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "System");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "FriendlyName",
                table: "Game");
        }
    }
}
