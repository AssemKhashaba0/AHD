using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Cemetery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSpaces",
                table: "Cemeteries");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Cemeteries");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Cemeteries");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cemeteries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cemeteries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "AvailableSpaces",
                table: "Cemeteries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Cemeteries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Cemeteries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
