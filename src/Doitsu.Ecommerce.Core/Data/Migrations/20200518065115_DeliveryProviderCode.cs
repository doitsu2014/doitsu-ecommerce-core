using Microsoft.EntityFrameworkCore.Migrations;

namespace Doitsu.Ecommerce.Core.Data.Migrations
{
    public partial class DeliveryProviderCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryProviderCode",
                table: "Orders",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryProviderReferenceLink",
                table: "Orders",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryProviderCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryProviderReferenceLink",
                table: "Orders");
        }
    }
}
