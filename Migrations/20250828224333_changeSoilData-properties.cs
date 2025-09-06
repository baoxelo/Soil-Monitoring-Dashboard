using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soil_Monitoring_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class changeSoilDataproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "6617ca4b-ee91-4b7d-abc3-6ba672668639");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "9c90edde-960e-4cdc-8bec-a2341f8295a4");

            migrationBuilder.RenameColumn(
                name: "Longtitude",
                table: "Sensors",
                newName: "Longitude");

            migrationBuilder.AlterColumn<decimal>(
                name: "PH",
                table: "SoilData",
                type: "Decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EC",
                table: "SoilData",
                type: "DECIMAL(10,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(6,3)");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "067fc46a-ede6-4b72-bcb4-931e093adb12", null, "User", "USER" },
                    { "862a3423-fa1e-4c48-b2b1-9a72677d0ce1", null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "067fc46a-ede6-4b72-bcb4-931e093adb12");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "862a3423-fa1e-4c48-b2b1-9a72677d0ce1");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Sensors",
                newName: "Longtitude");

            migrationBuilder.AlterColumn<decimal>(
                name: "PH",
                table: "SoilData",
                type: "decimal(10,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "Decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EC",
                table: "SoilData",
                type: "DECIMAL(6,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,4)");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6617ca4b-ee91-4b7d-abc3-6ba672668639", null, "Administrator", "ADMINISTRATOR" },
                    { "9c90edde-960e-4cdc-8bec-a2341f8295a4", null, "User", "USER" }
                });
        }
    }
}
