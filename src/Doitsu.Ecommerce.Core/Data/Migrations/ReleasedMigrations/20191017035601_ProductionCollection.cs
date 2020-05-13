using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class ProductionCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaviconUrl",
                table: "Brand",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductCollections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ThumbnailUrl = table.Column<string>(maxLength: 1000, nullable: true),
                    Slug = table.Column<string>(maxLength: 255, nullable: false),
                    IsFixed = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCollections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCollectionMappings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCollectionId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCollectionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ProductCollection__ProductCollMapping",
                        column: x => x.ProductCollectionId,
                        principalTable: "ProductCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Product__ProductCollMapping",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCollectionMappings_ProductId",
                table: "ProductCollectionMappings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "UQ__ProductCollectionId__ProductId",
                table: "ProductCollectionMappings",
                columns: new[] { "ProductCollectionId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__ProductCollection_Slug",
                table: "ProductCollections",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCollectionMappings");

            migrationBuilder.DropTable(
                name: "ProductCollections");

            migrationBuilder.DropColumn(
                name: "FaviconUrl",
                table: "Brand");
        }
    }
}
