using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class AddProductWeightAndWareHouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "Products",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "WareHouse",
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
                    table.PrimaryKey("PK_WareHouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WareHouse_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WareHouse_BrandId",
                table: "WareHouse",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Name",
                table: "WareHouse",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WareHouse");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Products");
        }
    }
}
