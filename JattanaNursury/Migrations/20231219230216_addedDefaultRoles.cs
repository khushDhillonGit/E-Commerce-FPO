using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JattanaNursury.Migrations
{
    public partial class addedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("058ec6cf-3246-4321-bf87-049391b11b96"), "2023-12-19 4:02:16 PM", "Admin", "admin" },
                    { new Guid("3b7a2946-53b9-4142-965c-a3ad484c9ed3"), "2023-12-19 4:02:16 PM", "Customer", "customer" },
                    { new Guid("58922adb-383c-4a48-9666-012ae1ee599c"), "2023-12-19 4:02:16 PM", "SuperAdmin", "superadmin" },
                    { new Guid("faa38780-60f2-4c7e-bdd0-9c7372ce3ddb"), "2023-12-19 4:02:16 PM", "Employee", "employee" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("058ec6cf-3246-4321-bf87-049391b11b96"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3b7a2946-53b9-4142-965c-a3ad484c9ed3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("58922adb-383c-4a48-9666-012ae1ee599c"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("faa38780-60f2-4c7e-bdd0-9c7372ce3ddb"));
        }
    }
}
