using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNGCICOpendatasoft_Extension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Messier",
                table: "NGCICOpendatasoft_Extension",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "NGCICOpendatasoft_Extension",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                table: "NGCICOpendatasoft_Extension",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Messier",
                table: "NGCICOpendatasoft",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "NGCICOpendatasoft_Extension");

            migrationBuilder.DropColumn(
                name: "PageNumber",
                table: "NGCICOpendatasoft_Extension");

            migrationBuilder.AlterColumn<string>(
                name: "Messier",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Messier",
                table: "NGCICOpendatasoft",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);
        }
    }
}
