using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class Add_DatabaseInitialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatabaseInitialization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Is_SourceType_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_NGC2000_UKTemporarilySource_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_NameObject_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_Constellation_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_NGC2000_UKTemporarily_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_CollinderCatalog_Temporarily_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_CollinderCatalog_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_NGCWikipedia_TemporarilySource_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_NGCWikipedia_ExtensionTemporarilySource_Executed = table.Column<bool>(type: "bit", nullable: false),
                    Is_NGCICOpendatasoft_Source_Executed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseInitialization", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatabaseInitialization");
        }
    }
}
