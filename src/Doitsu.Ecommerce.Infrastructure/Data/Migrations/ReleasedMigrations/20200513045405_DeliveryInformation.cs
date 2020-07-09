using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Infrastructure.Data.Migrations
{
    public partial class DeliveryInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "Products",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryCity",
                table: "Orders",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryCountry",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryDistrict",
                table: "Orders",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFees",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryProvider",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryWard",
                table: "Orders",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProofImageUrl",
                table: "Orders",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentValue",
                table: "Orders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RefernceDeliveryInformationId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "GalleryItems",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "GalleryItems",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "GalleryItems",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Galleries",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Galleries",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateTable(
                name: "DeliveryInformation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 125, nullable: true),
                    Address = table.Column<string>(maxLength: 255, nullable: true),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    Country = table.Column<string>(maxLength: 60, nullable: true),
                    City = table.Column<string>(maxLength: 60, nullable: true),
                    District = table.Column<string>(maxLength: 30, nullable: true),
                    Ward = table.Column<string>(maxLength: 30, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 15, nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryInformation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WareHouses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    City = table.Column<string>(maxLength: 64, nullable: false),
                    District = table.Column<string>(maxLength: 64, nullable: false),
                    Ward = table.Column<string>(maxLength: 32, nullable: false),
                    Address = table.Column<string>(maxLength: 256, nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Vers = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WareHouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WareHouses_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RefernceDeliveryInformationId",
                table: "Orders",
                column: "RefernceDeliveryInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryInformation_UserId",
                table: "DeliveryInformation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouses_BrandId",
                table: "WareHouses",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Name",
                table: "WareHouses",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryInformation_RefernceDeliveryInformationId",
                table: "Orders",
                column: "RefernceDeliveryInformationId",
                principalTable: "DeliveryInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryInformation_RefernceDeliveryInformationId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DeliveryInformation");

            migrationBuilder.DropTable(
                name: "WareHouses");

            migrationBuilder.DropIndex(
                name: "IX_Orders_RefernceDeliveryInformationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeliveryCity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryCountry",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryDistrict",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryFees",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryProvider",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryWard",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentProofImageUrl",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentValue",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RefernceDeliveryInformationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "GalleryItems");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "GalleryItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "GalleryItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Galleries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Galleries",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }
    }
}
