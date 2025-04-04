using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeCamposActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "activitydate",
                schema: "public",
                table: "activities",
                newName: "activitystartdate");

            migrationBuilder.AddColumn<DateTime>(
                name: "activityenddate",
                schema: "public",
                table: "activities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activityenddate",
                schema: "public",
                table: "activities");

            migrationBuilder.RenameColumn(
                name: "activitystartdate",
                schema: "public",
                table: "activities",
                newName: "activitydate");
        }
    }
}
