using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCollections.Migrations
{
    public partial class separated_gameDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameDetails",
                table: "Game");

            migrationBuilder.CreateTable(
                name: "GameDetails",
                columns: table => new
                {
                    GameDetailsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SteamApID = table.Column<int>(nullable: true),
                    IGDBId = table.Column<int>(nullable: true),
                    IDDBData = table.Column<string>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameDetails", x => x.GameDetailsID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameDetails");

            migrationBuilder.AddColumn<string>(
                name: "GameDetails",
                table: "Game",
                nullable: true);
        }
    }
}
