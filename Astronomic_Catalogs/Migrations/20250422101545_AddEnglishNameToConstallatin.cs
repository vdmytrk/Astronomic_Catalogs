using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class AddEnglishNameToConstallatin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Limit_Ang_Diameter",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Limit_Ang_Diameter",
                table: "NGCICOpendatasoft",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "English_name",
                table: "Constellation",
                type: "nvarchar(30)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft_Extension");

            migrationBuilder.DropColumn(
                name: "Limit_Ang_Diameter",
                table: "NGCICOpendatasoft_Extension");

            migrationBuilder.DropColumn(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft");

            migrationBuilder.DropColumn(
                name: "Limit_Ang_Diameter",
                table: "NGCICOpendatasoft");

            migrationBuilder.DropColumn(
                name: "English_name",
                table: "Constellation");
        }
    }
}
