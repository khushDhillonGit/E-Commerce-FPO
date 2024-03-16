using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    public partial class addedUserImageUrlField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
       

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

          
        }
    }
}
