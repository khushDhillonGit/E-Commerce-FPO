using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgresqlMigrations.Migrations
{
    public partial class totalPriceInProductOrderGet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "CustomerOrderProducts");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "CartProducts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "OrderProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "CustomerOrderProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "CartProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
