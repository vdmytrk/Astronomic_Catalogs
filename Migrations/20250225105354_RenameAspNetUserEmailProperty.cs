using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class RenameAspNetUserEmailProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountEmailSent",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "LastEmailSent",
                table: "AspNetUsers",
                newName: "LastRegisterEmailSent");

            migrationBuilder.AddColumn<int>(
                name: "CountRegisterEmailSent",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountRegisterEmailSent",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "LastRegisterEmailSent",
                table: "AspNetUsers",
                newName: "LastEmailSent");

            migrationBuilder.AddColumn<int>(
                name: "CountEmailSent",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
