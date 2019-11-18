using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class InitAttributesToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dynamic01",
                table: "Orders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dynamic02",
                table: "Orders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dynamic03",
                table: "Orders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dynamic04",
                table: "Orders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dynamic05",
                table: "Orders",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dynamic01",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Dynamic02",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Dynamic03",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Dynamic04",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Dynamic05",
                table: "Orders");
        }
    }
}
