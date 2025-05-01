using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFildsTipeOfNGCICOpendatasoft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OpenngcNotes",
                table: "NGCICOpendatasoft_Extension",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(330)",
                oldMaxLength: 330,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft_Extension",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(110)",
                oldMaxLength: 110,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OpenngcNotes",
                table: "NGCICOpendatasoft",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(330)",
                oldMaxLength: 330,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(110)",
                oldMaxLength: 110,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OpenngcNotes",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(330)",
                maxLength: 330,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(110)",
                maxLength: 110,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OpenngcNotes",
                table: "NGCICOpendatasoft",
                type: "nvarchar(330)",
                maxLength: 330,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NedNotes",
                table: "NGCICOpendatasoft",
                type: "nvarchar(110)",
                maxLength: 110,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);
        }
    }
}
