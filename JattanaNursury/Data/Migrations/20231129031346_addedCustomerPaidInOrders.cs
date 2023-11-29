using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JattanaNursury.Data.Migrations
{
    public partial class addedCustomerPaidInOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPaid",
                table: "Orders",
                newName: "FullyPaid");

            migrationBuilder.AddColumn<decimal>(
                name: "PaidByCustomer",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidByCustomer",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "FullyPaid",
                table: "Orders",
                newName: "IsPaid");
        }
    }
}
