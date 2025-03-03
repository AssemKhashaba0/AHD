using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryLocationOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryLocationId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DeliveryLocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryLocationId",
                table: "Orders",
                column: "DeliveryLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryLocations_DeliveryLocationId",
                table: "Orders",
                column: "DeliveryLocationId",
                principalTable: "DeliveryLocations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryLocations_DeliveryLocationId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryLocationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryLocationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DeliveryLocations");
        }
    }
}
