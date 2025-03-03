using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Order1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryManId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryManId",
                table: "Orders",
                column: "DeliveryManId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_DeliveryManId",
                table: "Orders",
                column: "DeliveryManId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_DeliveryManId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryManId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryManId",
                table: "Orders");
        }
    }
}
