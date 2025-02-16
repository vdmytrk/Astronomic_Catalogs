using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "AspNetRole",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetRole", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUser",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //        TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            //        LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            //        LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            //        AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUser", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetRoleClaim",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetRoleClaim", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AspNetRoleClaim_AspNetRole_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "AspNetRole",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserClaim",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserClaim", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AspNetUserClaim_AspNetUser_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUser",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserLogin",
            //    columns: table => new
            //    {
            //        LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserLogin", x => new { x.LoginProvider, x.ProviderKey });
            //        table.ForeignKey(
            //            name: "FK_AspNetUserLogin_AspNetUser_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUser",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserRole",
            //    columns: table => new
            //    {
            //        UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserRole", x => new { x.UserId, x.RoleId });
            //        table.ForeignKey(
            //            name: "FK_AspNetUserRole_AspNetRole_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "AspNetRole",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_AspNetUserRole_AspNetUser_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUser",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserToken",
            //    columns: table => new
            //    {
            //        UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserToken", x => new { x.UserId, x.LoginProvider, x.Name });
            //        table.ForeignKey(
            //            name: "FK_AspNetUserToken_AspNetUser_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUser",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_AspNetRoleClaim_RoleId",
            //    table: "AspNetRoleClaim",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AspNetUserClaim_UserId",
            //    table: "AspNetUserClaim",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AspNetUserLogin_UserId",
            //    table: "AspNetUserLogin",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AspNetUserRole_RoleId",
            //    table: "AspNetUserRole",
            //    column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "AspNetRoleClaim");

            //migrationBuilder.DropTable(
            //    name: "AspNetUserClaim");

            //migrationBuilder.DropTable(
            //    name: "AspNetUserLogin");

            //migrationBuilder.DropTable(
            //    name: "AspNetUserRole");

            //migrationBuilder.DropTable(
            //    name: "AspNetUserToken");

            //migrationBuilder.DropTable(
            //    name: "AspNetRole");

            //migrationBuilder.DropTable(
            //    name: "AspNetUser");
        }
    }
}
