using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamId = table.Column<string>(nullable: false),
                    DefaultMilestonesToTrack = table.Column<string>(nullable: true),
                    MemberId = table.Column<string>(nullable: true),
                    Organization = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamId);
                });

            migrationBuilder.CreateTable(
                name: "CostMarker",
                columns: table => new
                {
                    CostMarkerId = table.Column<string>(nullable: false),
                    Factor = table.Column<double>(nullable: false),
                    TeamId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostMarker", x => x.CostMarkerId);
                    table.ForeignKey(
                        name: "FK_CostMarker_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<string>(nullable: false),
                    TeamId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Members_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Repo",
                columns: table => new
                {
                    RepoId = table.Column<string>(nullable: false),
                    TeamId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repo", x => x.RepoId);
                    table.ForeignKey(
                        name: "FK_Repo_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostMarker_TeamId",
                table: "CostMarker",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TeamId",
                table: "Members",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Repo_TeamId",
                table: "Repo",
                column: "TeamId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "CostMarker");

            migrationBuilder.DropTable(
                name: "Repo");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
