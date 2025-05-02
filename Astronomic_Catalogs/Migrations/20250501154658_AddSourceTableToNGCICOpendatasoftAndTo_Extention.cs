using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceTableToNGCICOpendatasoftAndTo_Extention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceTable",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceTable",
                table: "NGCICOpendatasoft",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceTable",
                table: "NGCICOpendatasoft_Extension");

            migrationBuilder.DropColumn(
                name: "SourceTable",
                table: "NGCICOpendatasoft");
        }
    }
}
