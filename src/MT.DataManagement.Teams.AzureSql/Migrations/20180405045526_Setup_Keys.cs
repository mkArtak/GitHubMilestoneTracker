using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Migrations
{
    public partial class Setup_Keys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostMarker_Teams_TeamId",
                table: "CostMarker");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostMarker",
                table: "CostMarker");

            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "Teams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberTeamId",
                table: "Teams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepoId",
                table: "Teams",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TeamId",
                table: "Members",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInReports",
                table: "Members",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TeamId",
                table: "CostMarker",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                columns: new[] { "MemberId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostMarker",
                table: "CostMarker",
                columns: new[] { "CostMarkerId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_RepoId",
                table: "Teams",
                column: "RepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_MemberId_MemberTeamId",
                table: "Teams",
                columns: new[] { "MemberId", "MemberTeamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CostMarker_Teams_TeamId",
                table: "CostMarker",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Repo_RepoId",
                table: "Teams",
                column: "RepoId",
                principalTable: "Repo",
                principalColumn: "RepoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Members_MemberId_MemberTeamId",
                table: "Teams",
                columns: new[] { "MemberId", "MemberTeamId" },
                principalTable: "Members",
                principalColumns: new[] { "MemberId", "TeamId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostMarker_Teams_TeamId",
                table: "CostMarker");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Repo_RepoId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Members_MemberId_MemberTeamId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_RepoId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_MemberId_MemberTeamId",
                table: "Teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostMarker",
                table: "CostMarker");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "MemberTeamId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "RepoId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IncludeInReports",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "TeamId",
                table: "Members",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "TeamId",
                table: "CostMarker",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostMarker",
                table: "CostMarker",
                column: "CostMarkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostMarker_Teams_TeamId",
                table: "CostMarker",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
