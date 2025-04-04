using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeCamposEventETicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                schema: "public",
                table: "events");

            migrationBuilder.RenameColumn(
                name: "eventdate",
                schema: "public",
                table: "events",
                newName: "eventstartdate");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "public",
                table: "eventtickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "eventenddate",
                schema: "public",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "public",
                table: "eventtickets");

            migrationBuilder.DropColumn(
                name: "eventenddate",
                schema: "public",
                table: "events");

            migrationBuilder.RenameColumn(
                name: "eventstartdate",
                schema: "public",
                table: "events",
                newName: "eventdate");

            migrationBuilder.AddColumn<decimal>(
                name: "price",
                schema: "public",
                table: "events",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
