using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class InitProductVariant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductOptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOptions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    Sku = table.Column<string>(maxLength: 256, nullable: false),
                    AnotherPrice = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    AnotherDiscount = table.Column<float>(nullable: false, defaultValue: 0f),
                    InventoryQuantity = table.Column<long>(nullable: false, defaultValue: -1L),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductOptionValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductOptionId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 256, nullable: true),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOptionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOptionValues_ProductOptions_ProductOptionId",
                        column: x => x.ProductOptionId,
                        principalTable: "ProductOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantOptionValues",
                columns: table => new
                {
                    ProductOptionId = table.Column<int>(nullable: false),
                    ProductVariantId = table.Column<int>(nullable: false),
                    ProductOptionValueId = table.Column<int>(nullable: false),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantOptionValues", x => new { x.ProductOptionId, x.ProductVariantId });
                    table.ForeignKey(
                        name: "FK_ProductVariantOptionValues_ProductOptions_ProductOptionId",
                        column: x => x.ProductOptionId,
                        principalTable: "ProductOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariantOptionValues_ProductOptionValues_ProductOptionValueId",
                        column: x => x.ProductOptionValueId,
                        principalTable: "ProductOptionValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariantOptionValues_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOptions_Name",
                table: "ProductOptions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductOptions_ProductId",
                table: "ProductOptions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOptionValues_ProductOptionId",
                table: "ProductOptionValues",
                column: "ProductOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantOptionValues_ProductOptionValueId",
                table: "ProductVariantOptionValues",
                column: "ProductOptionValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantOptionValues_ProductVariantId",
                table: "ProductVariantOptionValues",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_Sku",
                table: "ProductVariants",
                column: "Sku",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariantOptionValues");

            migrationBuilder.DropTable(
                name: "ProductOptionValues");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "ProductOptions");
        }
    }
}
