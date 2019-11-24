using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class UpdateProductVariantAndOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductOptions_Name",
                table: "ProductOptions");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ProductVariants",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductVariants",
                nullable: false,
                defaultValue: 1);

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

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductOptionValues",
                nullable: false,
                defaultValue: 1);

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

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ProductVariantOptionValues");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ProductOptionValues");

            migrationBuilder.DropColumn(
                name: "Status",
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

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "OrderItems");

            migrationBuilder.AddColumn<int>(
                name: "Sku",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductOptions_Name",
                table: "ProductOptions",
                column: "Name",
                unique: true);
        }
    }
}
