using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    MosqueId = table.Column<int>(type: "int", nullable: true),
                    CemeteryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_carts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_carts_Cemeteries_CemeteryId",
                        column: x => x.CemeteryId,
                        principalTable: "Cemeteries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_carts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_carts_Mosques_MosqueId",
                        column: x => x.MosqueId,
                        principalTable: "Mosques",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_carts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_carts_ApplicationUserId",
                table: "carts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_carts_CemeteryId",
                table: "carts",
                column: "CemeteryId");

            migrationBuilder.CreateIndex(
                name: "IX_carts_CityId",
                table: "carts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_carts_MosqueId",
                table: "carts",
                column: "MosqueId");

            migrationBuilder.CreateIndex(
                name: "IX_carts_ProductId",
                table: "carts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carts");
        }
    }
}
