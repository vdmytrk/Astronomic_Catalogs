using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class AspNetUser_YearOfBirth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearOfBirth",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfBirth",
                table: "AspNetUsers");
        }
    }
}
