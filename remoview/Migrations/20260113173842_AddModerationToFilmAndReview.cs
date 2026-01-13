using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace remoview.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationToFilmAndReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModeratedAtUtc",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModeratedByUserId",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationNote",
                table: "Reviews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Films",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Films",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModeratedAtUtc",
                table: "Films",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModeratedByUserId",
                table: "Films",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationNote",
                table: "Films",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Films",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // ✅ Favorites zaten DB'de var → bu migration'da dokunmuyoruz.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ✅ Favorites'a dokunma (zaten önceden vardı)

            migrationBuilder.DropColumn(
                name: "ModeratedAtUtc",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ModeratedByUserId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ModerationNote",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "ModeratedAtUtc",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "ModeratedByUserId",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "ModerationNote",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Films");
        }
    }
}
