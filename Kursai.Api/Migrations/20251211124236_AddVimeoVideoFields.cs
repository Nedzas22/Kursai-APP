using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursai.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVimeoVideoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VideoDurationSeconds",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoThumbnailUrl",
                table: "Courses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VimeoEmbedUrl",
                table: "Courses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VimeoVideoId",
                table: "Courses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "VideoDurationSeconds", "VideoThumbnailUrl", "VimeoEmbedUrl", "VimeoVideoId" },
                values: new object[] { new DateTime(2025, 12, 1, 12, 42, 35, 648, DateTimeKind.Utc).AddTicks(2180), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "VideoDurationSeconds", "VideoThumbnailUrl", "VimeoEmbedUrl", "VimeoVideoId" },
                values: new object[] { new DateTime(2025, 12, 6, 12, 42, 35, 648, DateTimeKind.Utc).AddTicks(2673), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "VideoDurationSeconds", "VideoThumbnailUrl", "VimeoEmbedUrl", "VimeoVideoId" },
                values: new object[] { new DateTime(2025, 12, 8, 12, 42, 35, 648, DateTimeKind.Utc).AddTicks(2685), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 11, 12, 42, 35, 646, DateTimeKind.Utc).AddTicks(5688), "$2a$11$WXh4XP8M7P2plsaZ4ZvDvezUlIF5kV57EeM0a8vhqhrPSdv6JTZdq" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoDurationSeconds",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VideoThumbnailUrl",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VimeoEmbedUrl",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VimeoVideoId",
                table: "Courses");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 13, 19, 11, 539, DateTimeKind.Utc).AddTicks(3798));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 13, 19, 11, 539, DateTimeKind.Utc).AddTicks(4280));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 13, 19, 11, 539, DateTimeKind.Utc).AddTicks(4291));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 10, 13, 19, 11, 537, DateTimeKind.Utc).AddTicks(7849), "$2a$11$v18kdesWSeoW7gprZCzyHudPhZ3nAap6yUpzLQw3A/BjF.MKYFz2." });
        }
    }
}
