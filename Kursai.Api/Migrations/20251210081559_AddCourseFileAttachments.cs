using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursai.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseFileAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                table: "Courses",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "AttachmentFileSize",
                table: "Courses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileType",
                table: "Courses",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileUrl",
                table: "Courses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AttachmentFileName", "AttachmentFileSize", "AttachmentFileType", "AttachmentFileUrl", "CreatedAt" },
                values: new object[] { null, null, null, null, new DateTime(2025, 11, 30, 8, 15, 57, 888, DateTimeKind.Utc).AddTicks(5763) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AttachmentFileName", "AttachmentFileSize", "AttachmentFileType", "AttachmentFileUrl", "CreatedAt" },
                values: new object[] { null, null, null, null, new DateTime(2025, 12, 5, 8, 15, 57, 888, DateTimeKind.Utc).AddTicks(6614) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AttachmentFileName", "AttachmentFileSize", "AttachmentFileType", "AttachmentFileUrl", "CreatedAt" },
                values: new object[] { null, null, null, null, new DateTime(2025, 12, 7, 8, 15, 57, 888, DateTimeKind.Utc).AddTicks(6627) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 10, 8, 15, 57, 886, DateTimeKind.Utc).AddTicks(1414), "$2a$11$2FXVMb.hpGjP4Go2.gY4pu/tWQF1zwSnGWnDBKbjE9mL4yNMMlULu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AttachmentFileSize",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AttachmentFileType",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AttachmentFileUrl",
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
    }
}
