using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCollections.Migrations
{
    public partial class multiple_users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Game",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game_UserId",
                table: "Game",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_AspNetUsers_UserId",
                table: "Game",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_AspNetUsers_UserId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_UserId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Game");
        }
    }
}
