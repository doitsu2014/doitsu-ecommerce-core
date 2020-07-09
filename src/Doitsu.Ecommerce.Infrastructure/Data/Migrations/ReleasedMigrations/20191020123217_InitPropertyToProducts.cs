using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Infrastructure.Data.Migrations
{
    public partial class InitPropertyToProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Property",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Property",
                table: "Products");
        }
    }
}
