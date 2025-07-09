using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanetarySystemsCatalog_UpdateOthers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NLog",
                table: "NLogApplicationCode");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "NGCICOpendatasoft_Extension");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "NGCICOpendatasoft");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "NASAExoplanetCatalog");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "CollinderCatalog");

            migrationBuilder.RenameColumn(
                name: "PageNumber",
                table: "NGCICOpendatasoft_Extension",
                newName: "RowOnPage");

            migrationBuilder.RenameColumn(
                name: "PageNumber",
                table: "NGCICOpendatasoft",
                newName: "RowOnPage");

            migrationBuilder.RenameColumn(
                name: "PageNumber",
                table: "NASAExoplanetCatalog",
                newName: "RowOnPage");

            migrationBuilder.RenameColumn(
                name: "PageNumber",
                table: "CollinderCatalog",
                newName: "RowOnPage");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Logged",
                table: "NLogApplicationCode",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "NLogApplicationCode",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<double>(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft_Extension",
                type: "float",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft",
                type: "float",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Right_ascension_S",
                table: "CollinderCatalog",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Right_ascension_M",
                table: "CollinderCatalog",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Declination_S",
                table: "CollinderCatalog",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "App_Mag",
                table: "CollinderCatalog",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Ang_Diameter_Max",
                table: "CollinderCatalog",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NLogApplicationCode",
                table: "NLogApplicationCode",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PlanetarySystemsCatalog",
                columns: table => new
                {
                    Hostname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HdName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HipName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GaiaId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StSpectype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StTeff = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StRad = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StMass = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StMet = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StMetratio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StLum = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StAge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SyDist = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StLumSunAbsol = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HabitablZone = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlLetter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlRade = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlRadJ = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlMasse = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlMassJ = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlOrbsmax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RowOnPage = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanetarySystemsCatalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NLogApplicationCode",
                table: "NLogApplicationCode");

            migrationBuilder.RenameColumn(
                name: "RowOnPage",
                table: "NGCICOpendatasoft_Extension",
                newName: "PageNumber");

            migrationBuilder.RenameColumn(
                name: "RowOnPage",
                table: "NGCICOpendatasoft",
                newName: "PageNumber");

            migrationBuilder.RenameColumn(
                name: "RowOnPage",
                table: "NASAExoplanetCatalog",
                newName: "PageNumber");

            migrationBuilder.RenameColumn(
                name: "RowOnPage",
                table: "CollinderCatalog",
                newName: "PageNumber");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Logged",
                table: "NLogApplicationCode",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "NLogApplicationCode",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft_Extension",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "NGCICOpendatasoft_Extension",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ang_Diameter",
                table: "NGCICOpendatasoft",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "NGCICOpendatasoft",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "NASAExoplanetCatalog",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Right_ascension_S",
                table: "CollinderCatalog",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Right_ascension_M",
                table: "CollinderCatalog",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Declination_S",
                table: "CollinderCatalog",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "App_Mag",
                table: "CollinderCatalog",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Ang_Diameter_Max",
                table: "CollinderCatalog",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "CollinderCatalog",
                type: "int",
                nullable: true,
                defaultValue: 133);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NLog",
                table: "NLogApplicationCode",
                column: "Id");
        }
    }
}
