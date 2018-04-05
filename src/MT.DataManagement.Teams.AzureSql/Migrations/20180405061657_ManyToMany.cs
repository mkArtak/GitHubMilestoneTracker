using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Migrations
{
    public partial class ManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Repo_Teams_TeamId",
                table: "Repo");

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
                name: "PK_Repo",
                table: "Repo");

            migrationBuilder.DropIndex(
                name: "IX_Repo_TeamId",
                table: "Repo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_TeamId",
                table: "Members");

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
                name: "TeamId",
                table: "Repo");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Members");

            migrationBuilder.RenameTable(
                name: "Repo",
                newName: "Repos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Repos",
                table: "Repos",
                column: "RepoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "MemberId");

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    TeamId = table.Column<string>(nullable: false),
                    MemberId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => new { x.TeamId, x.MemberId });
                    table.UniqueConstraint("AK_TeamMember_MemberId_TeamId", x => new { x.MemberId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_TeamMember_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamRepo",
                columns: table => new
                {
                    TeamId = table.Column<string>(nullable: false),
                    RepoId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRepo", x => new { x.TeamId, x.RepoId });
                    table.UniqueConstraint("AK_TeamRepo_RepoId_TeamId", x => new { x.RepoId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_TeamRepo_Repos_RepoId",
                        column: x => x.RepoId,
                        principalTable: "Repos",
                        principalColumn: "RepoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamRepo_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "TeamRepo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Repos",
                table: "Repos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.RenameTable(
                name: "Repos",
                newName: "Repo");

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

            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "Repo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "Members",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Repo",
                table: "Repo",
                column: "RepoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                columns: new[] { "MemberId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_RepoId",
                table: "Teams",
                column: "RepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_MemberId_MemberTeamId",
                table: "Teams",
                columns: new[] { "MemberId", "MemberTeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Repo_TeamId",
                table: "Repo",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TeamId",
                table: "Members",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repo_Teams_TeamId",
                table: "Repo",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);

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
    }
}
