using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JattanaNursury.Data.Migrations
{
    public partial class addedNewFieldForLatePaymentInorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Orders");
        }
    }
}
