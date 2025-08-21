using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuleBankApp.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_outbox_messages_Status_CreatedAtUtc",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "Error",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "LastAttemptAtUtc",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "OccurredAtUtc",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "PublishAttempts",
                table: "outbox_messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "outbox_messages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "outbox_messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAttemptAtUtc",
                table: "outbox_messages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OccurredAtUtc",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "PublishAttempts",
                table: "outbox_messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_Status_CreatedAtUtc",
                table: "outbox_messages",
                columns: new[] { "Status", "CreatedAtUtc" });
        }
    }
}
