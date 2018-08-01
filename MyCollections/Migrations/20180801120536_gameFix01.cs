using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MyCollections.Migrations
{
    public partial class gameFix01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Store_StoreID",
                table: "Game");

            migrationBuilder.AlterColumn<int>(
                name: "StoreID",
                table: "Game",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "SteamApID",
                table: "Game",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Game",
                nullable: true,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "PlayedTime",
                table: "Game",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "BuyDate",
                table: "Game",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Store_StoreID",
                table: "Game",
                column: "StoreID",
                principalTable: "Store",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Store_StoreID",
                table: "Game");

            migrationBuilder.AlterColumn<int>(
                name: "StoreID",
                table: "Game",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SteamApID",
                table: "Game",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Game",
                nullable: false,
                oldClrType: typeof(float),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayedTime",
                table: "Game",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BuyDate",
                table: "Game",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Store_StoreID",
                table: "Game",
                column: "StoreID",
                principalTable: "Store",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
