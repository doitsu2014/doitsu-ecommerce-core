using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class InitOrderDynamicAndActiveProductOptionVariant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ProductVariantOptionValues",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ProductOptionValues",
                nullable: false,
                defaultValue: true);

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
                name: "Active",
                table: "ProductVariantOptionValues");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ProductOptionValues");

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
