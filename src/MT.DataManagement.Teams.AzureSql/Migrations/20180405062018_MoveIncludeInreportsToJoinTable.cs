using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Migrations
{
    public partial class MoveIncludeInreportsToJoinTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeInReports",
                table: "Members");

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInReports",
                table: "TeamMember",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeInReports",
                table: "TeamMember");

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInReports",
                table: "Members",
                nullable: false,
                defaultValue: false);
        }
    }
}
