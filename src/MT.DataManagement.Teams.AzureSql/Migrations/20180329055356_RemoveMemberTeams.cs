using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Migrations
{
    public partial class RemoveMemberTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Members_MemberId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_MemberId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Teams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "Teams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_MemberId",
                table: "Teams",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Members_MemberId",
                table: "Teams",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
