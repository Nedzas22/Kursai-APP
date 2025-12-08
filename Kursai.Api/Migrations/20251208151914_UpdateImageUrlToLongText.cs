using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursai.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageUrlToLongText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Courses",
                type: "LONGTEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 15, 19, 13, 315, DateTimeKind.Utc).AddTicks(2137));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 15, 19, 13, 315, DateTimeKind.Utc).AddTicks(2595));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 15, 19, 13, 315, DateTimeKind.Utc).AddTicks(2606));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 8, 15, 19, 13, 313, DateTimeKind.Utc).AddTicks(7055), "$2a$11$EwqIfXN7FL1xkopIH3sneOwFPm4WQ6atzD5kx8W9tRCQQ.S6boIOG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Courses",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "LONGTEXT")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 13, 47, 54, 324, DateTimeKind.Utc).AddTicks(4393));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 13, 47, 54, 324, DateTimeKind.Utc).AddTicks(4978));

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 13, 47, 54, 324, DateTimeKind.Utc).AddTicks(4990));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 8, 13, 47, 54, 322, DateTimeKind.Utc).AddTicks(5043), "$2a$11$KdsJxVd2umN04iqjXVYc7OaSVs2zEB876Z5YhjeH1d88kVtVV7XYm" });
        }
    }
}
