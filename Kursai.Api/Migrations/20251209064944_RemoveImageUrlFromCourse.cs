using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursai.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageUrlFromCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Courses");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 6, 49, 43, 4, DateTimeKind.Utc).AddTicks(4120));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 6, 49, 43, 4, DateTimeKind.Utc).AddTicks(5138));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 6, 49, 43, 4, DateTimeKind.Utc).AddTicks(5153));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 9, 6, 49, 43, 2, DateTimeKind.Utc).AddTicks(407), "$2a$11$Y1jcZxNmgRzGzE6mgDvBW.0sbMG9LGq2nIKKImoAtn2PNu3WpBa.i" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Courses",
                type: "LONGTEXT",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2025, 11, 28, 15, 19, 13, 315, DateTimeKind.Utc).AddTicks(2137), "dotnet_bot.png" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2025, 12, 3, 15, 19, 13, 315, DateTimeKind.Utc).AddTicks(2595), "dotnet_bot.png" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2025, 12, 5, 15, 19, 13, 315, DateTimeKind.Utc).AddTicks(2606), "dotnet_bot.png" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 8, 15, 19, 13, 313, DateTimeKind.Utc).AddTicks(7055), "$2a$11$EwqIfXN7FL1xkopIH3sneOwFPm4WQ6atzD5kx8W9tRCQQ.S6boIOG" });
        }
    }
}
