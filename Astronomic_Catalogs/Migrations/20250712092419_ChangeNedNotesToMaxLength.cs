using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNedNotesToMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft_Extension",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
