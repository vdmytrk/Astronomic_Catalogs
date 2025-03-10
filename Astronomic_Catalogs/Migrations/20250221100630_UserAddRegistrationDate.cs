using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class UserAddRegistrationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "AspNetUsers");
        }
    }
}
