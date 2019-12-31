using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class SummaryOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_ProductVariants_ProductVariantId",
                table: "PromotionDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "UserTransactions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductVariantId",
                table: "PromotionDetails",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PromotionDetails",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "DiscountPercent",
                table: "PromotionDetails",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "PromotionDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SummaryOrderId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SummaryOrderId",
                table: "Orders",
                column: "SummaryOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Orders_SummaryOrderId",
                table: "Orders",
                column: "SummaryOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_ProductVariants_ProductVariantId",
                table: "PromotionDetails",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Orders_SummaryOrderId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_ProductVariants_ProductVariantId",
                table: "PromotionDetails");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SummaryOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "PromotionDetails");

            migrationBuilder.DropColumn(
                name: "SummaryOrderId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "UserTransactions",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ProductVariantId",
                table: "PromotionDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PromotionDetails",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "DiscountPercent",
                table: "PromotionDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float));

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_ProductVariants_ProductVariantId",
                table: "PromotionDetails",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
