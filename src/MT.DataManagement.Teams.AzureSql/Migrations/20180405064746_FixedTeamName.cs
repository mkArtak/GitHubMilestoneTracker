using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Migrations
{
    public partial class FixedTeamName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Members_MemberId",
                table: "TeamMember");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamRepo_Repos_RepoId",
                table: "TeamRepo");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamRepo_Teams_TeamId",
                table: "TeamRepo");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamRepo_RepoId_TeamId",
                table: "TeamRepo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamRepo",
                table: "TeamRepo");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamMember_MemberId_TeamId",
                table: "TeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember");

            migrationBuilder.RenameTable(
                name: "TeamRepo",
                newName: "TeamRepos");

            migrationBuilder.RenameTable(
                name: "TeamMember",
                newName: "TeamMembers");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamRepos_RepoId_TeamId",
                table: "TeamRepos",
                columns: new[] { "RepoId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamRepos",
                table: "TeamRepos",
                columns: new[] { "TeamId", "RepoId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamMembers_MemberId_TeamId",
                table: "TeamMembers",
                columns: new[] { "MemberId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "MemberId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Members_MemberId",
                table: "TeamMembers",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamRepos_Repos_RepoId",
                table: "TeamRepos",
                column: "RepoId",
                principalTable: "Repos",
                principalColumn: "RepoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamRepos_Teams_TeamId",
                table: "TeamRepos",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Members_MemberId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamRepos_Repos_RepoId",
                table: "TeamRepos");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamRepos_Teams_TeamId",
                table: "TeamRepos");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamRepos_RepoId_TeamId",
                table: "TeamRepos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamRepos",
                table: "TeamRepos");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TeamMembers_MemberId_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.RenameTable(
                name: "TeamRepos",
                newName: "TeamRepo");

            migrationBuilder.RenameTable(
                name: "TeamMembers",
                newName: "TeamMember");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamRepo_RepoId_TeamId",
                table: "TeamRepo",
                columns: new[] { "RepoId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamRepo",
                table: "TeamRepo",
                columns: new[] { "TeamId", "RepoId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TeamMember_MemberId_TeamId",
                table: "TeamMember",
                columns: new[] { "MemberId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember",
                columns: new[] { "TeamId", "MemberId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Members_MemberId",
                table: "TeamMember",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamRepo_Repos_RepoId",
                table: "TeamRepo",
                column: "RepoId",
                principalTable: "Repos",
                principalColumn: "RepoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamRepo_Teams_TeamId",
                table: "TeamRepo",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
