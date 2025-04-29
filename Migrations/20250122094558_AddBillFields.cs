using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MR_power.Migrations
{
    /// <inheritdoc />
    public partial class AddBillFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "Stook",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "BillNumber",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VIN",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillNumber",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "VIN",
                table: "Bill");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "Stook",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);
        }
    }
}
