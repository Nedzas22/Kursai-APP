using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursai.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVimeoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentFileType",
                table: "Courses",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentFileName",
                table: "Courses",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<byte[]>(
                name: "AttachmentFile",
                table: "Courses",
                type: "varbinary(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AttachmentFile", "CreatedAt" },
                values: new object[] { null, new DateTime(2025, 12, 1, 13, 26, 16, 916, DateTimeKind.Utc).AddTicks(5047) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AttachmentFile", "CreatedAt" },
                values: new object[] { null, new DateTime(2025, 12, 6, 13, 26, 16, 916, DateTimeKind.Utc).AddTicks(5785) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AttachmentFile", "CreatedAt" },
                values: new object[] { null, new DateTime(2025, 12, 8, 13, 26, 16, 916, DateTimeKind.Utc).AddTicks(5796) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 11, 13, 26, 16, 914, DateTimeKind.Utc).AddTicks(7116), "$2a$11$qrlSK/fCZL8NiLqUVYgVdu9sgigMGBFgX16hQ79IjhnjLzn4gx1tG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFile",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentFileType",
                table: "Courses",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentFileName",
                table: "Courses",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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
    }
}
