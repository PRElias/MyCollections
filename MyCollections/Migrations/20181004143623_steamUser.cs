using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCollections.Migrations
{
    public partial class steamUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "steamUser",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "steamUser",
                table: "AspNetUsers");
        }
    }
}
