using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomic_Catalogs.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollinderCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Namber_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NameOtherCat = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Constellation = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Right_ascension = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Right_ascension_H = table.Column<int>(type: "int", nullable: false),
                    Right_ascension_M = table.Column<float>(type: "real", nullable: false),
                    Right_ascension_S = table.Column<float>(type: "real", nullable: false),
                    Declination = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    NS = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Declination_D = table.Column<int>(type: "int", nullable: false),
                    Declination_M = table.Column<int>(type: "int", nullable: false),
                    Declination_S = table.Column<float>(type: "real", nullable: false),
                    App_Mag = table.Column<float>(type: "real", nullable: true),
                    App_Mag_Flag = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    CountStars = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountStars_ToFinding = table.Column<int>(type: "int", nullable: true),
                    Ang_Diameter_OLD = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Ang_Diameter_NEW = table.Column<float>(type: "real", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageNumber = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    PageCount = table.Column<int>(type: "int", nullable: true, defaultValue: 133)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollinderCatalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Constellation",
                columns: table => new
                {
                    Short_name = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Latine_name_Nominative_case = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Latine_name_Genitive_case = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Ukraine_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Area = table.Column<int>(type: "int", nullable: false),
                    Number_stars = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constellation", x => x.Short_name);
                });

            migrationBuilder.CreateTable(
                name: "DateTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActualDateProperty = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogProcFunc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FuncProc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Line = table.Column<int>(type: "int", nullable: false),
                    ErrorNumber = table.Column<int>(type: "int", nullable: false),
                    ErrorSeverity = table.Column<int>(type: "int", nullable: true),
                    ErrorState = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogProcFunc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NameObject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Object = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameObject", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NASAExoplanetCatalog",
                columns: table => new
                {
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pl_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hostname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pl_letter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hd_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hip_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tic_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gaia_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Default_flag = table.Column<int>(type: "int", nullable: false),
                    Sy_snum = table.Column<int>(type: "int", nullable: false),
                    Sy_pnum = table.Column<int>(type: "int", nullable: false),
                    Sy_mnum = table.Column<int>(type: "int", nullable: false),
                    Cb_flag = table.Column<int>(type: "int", nullable: false),
                    Discoverymethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disc_year = table.Column<int>(type: "int", nullable: false),
                    Disc_refname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disc_pubdate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Disc_locale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disc_facility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disc_telescope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disc_instrument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rv_flag = table.Column<int>(type: "int", nullable: false),
                    Pul_flag = table.Column<int>(type: "int", nullable: false),
                    Ptv_flag = table.Column<int>(type: "int", nullable: false),
                    Tran_flag = table.Column<int>(type: "int", nullable: false),
                    Ast_flag = table.Column<int>(type: "int", nullable: false),
                    Obm_flag = table.Column<int>(type: "int", nullable: false),
                    Micro_flag = table.Column<int>(type: "int", nullable: false),
                    Etv_flag = table.Column<int>(type: "int", nullable: false),
                    Ima_flag = table.Column<int>(type: "int", nullable: false),
                    Dkin_flag = table.Column<int>(type: "int", nullable: false),
                    Soltype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pl_controv_flag = table.Column<int>(type: "int", nullable: false),
                    Pl_refname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pl_orbper = table.Column<float>(type: "real", nullable: false),
                    Pl_orbpererr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbpererr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbperlim = table.Column<float>(type: "real", nullable: false),
                    Pl_orbsmax = table.Column<float>(type: "real", nullable: false),
                    Pl_orbsmaxerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbsmaxerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbsmaxlim = table.Column<float>(type: "real", nullable: false),
                    Pl_rade = table.Column<float>(type: "real", nullable: false),
                    Pl_radeerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_radeerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_radelim = table.Column<float>(type: "real", nullable: false),
                    Pl_radj = table.Column<float>(type: "real", nullable: false),
                    Pl_radjerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_radjerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_radjlim = table.Column<float>(type: "real", nullable: false),
                    Pl_masse = table.Column<float>(type: "real", nullable: false),
                    Pl_masseerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_masseerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_masselim = table.Column<float>(type: "real", nullable: false),
                    Pl_massj = table.Column<float>(type: "real", nullable: false),
                    Pl_massjerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_massjerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_massjlim = table.Column<float>(type: "real", nullable: false),
                    Pl_msinie = table.Column<float>(type: "real", nullable: false),
                    Pl_msinieerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_msinieerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_msinielim = table.Column<float>(type: "real", nullable: false),
                    Pl_msinij = table.Column<float>(type: "real", nullable: false),
                    Pl_msinijerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_msinijerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_msinijlim = table.Column<float>(type: "real", nullable: false),
                    Pl_cmasse = table.Column<float>(type: "real", nullable: false),
                    Pl_cmasseerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_cmasseerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_cmasselim = table.Column<float>(type: "real", nullable: false),
                    Pl_cmassj = table.Column<float>(type: "real", nullable: false),
                    Pl_cmassjerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_cmassjerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_cmassjlim = table.Column<float>(type: "real", nullable: false),
                    Pl_bmasse = table.Column<float>(type: "real", nullable: false),
                    Pl_bmasseerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_bmasseerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_bmasselim = table.Column<float>(type: "real", nullable: false),
                    Pl_bmassj = table.Column<float>(type: "real", nullable: false),
                    Pl_bmassjerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_bmassjerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_bmassjlim = table.Column<float>(type: "real", nullable: false),
                    Pl_bmassprov = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pl_dens = table.Column<float>(type: "real", nullable: false),
                    Pl_denserr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_denserr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_denslim = table.Column<float>(type: "real", nullable: false),
                    Pl_orbeccen = table.Column<float>(type: "real", nullable: false),
                    Pl_orbeccenerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbeccenerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbeccenlim = table.Column<float>(type: "real", nullable: false),
                    Pl_insol = table.Column<float>(type: "real", nullable: false),
                    Pl_insolerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_insolerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_insollim = table.Column<float>(type: "real", nullable: false),
                    Pl_eqt = table.Column<float>(type: "real", nullable: false),
                    Pl_eqterr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_eqterr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_eqtlim = table.Column<float>(type: "real", nullable: false),
                    Pl_orbincl = table.Column<float>(type: "real", nullable: false),
                    Pl_orbinclerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbinclerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbincllim = table.Column<float>(type: "real", nullable: false),
                    Pl_tranmid = table.Column<float>(type: "real", nullable: false),
                    Pl_tranmiderr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_tranmiderr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_tranmidlim = table.Column<float>(type: "real", nullable: false),
                    Pl_tsystemref = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ttv_flag = table.Column<int>(type: "int", nullable: false),
                    Pl_imppar = table.Column<float>(type: "real", nullable: false),
                    Pl_impparerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_impparerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_impparlim = table.Column<float>(type: "real", nullable: false),
                    Pl_trandep = table.Column<float>(type: "real", nullable: false),
                    Pl_trandeperr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_trandeperr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_trandeplim = table.Column<float>(type: "real", nullable: false),
                    Pl_trandur = table.Column<float>(type: "real", nullable: false),
                    Pl_trandurerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_trandurerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_trandurlim = table.Column<float>(type: "real", nullable: false),
                    Pl_ratdor = table.Column<float>(type: "real", nullable: false),
                    Pl_ratdorerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_ratdorerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_ratdorlim = table.Column<float>(type: "real", nullable: false),
                    Pl_ratror = table.Column<float>(type: "real", nullable: false),
                    Pl_ratrorerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_ratrorerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_ratrorlim = table.Column<float>(type: "real", nullable: false),
                    Pl_occdep = table.Column<float>(type: "real", nullable: false),
                    Pl_occdeperr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_occdeperr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_occdeplim = table.Column<float>(type: "real", nullable: false),
                    Pl_orbtper = table.Column<float>(type: "real", nullable: false),
                    Pl_orbtpererr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbtpererr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_orbtperlim = table.Column<float>(type: "real", nullable: false),
                    Pl_orblper = table.Column<float>(type: "real", nullable: false),
                    Pl_orblpererr1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pl_orblpererr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_orblperlim = table.Column<float>(type: "real", nullable: false),
                    Pl_rvamp = table.Column<float>(type: "real", nullable: false),
                    Pl_rvamperr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_rvamperr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_rvamplim = table.Column<float>(type: "real", nullable: false),
                    Pl_projobliq = table.Column<float>(type: "real", nullable: false),
                    Pl_projobliqerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_projobliqerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_projobliqlim = table.Column<float>(type: "real", nullable: false),
                    Pl_trueobliq = table.Column<float>(type: "real", nullable: false),
                    Pl_trueobliqerr1 = table.Column<float>(type: "real", nullable: false),
                    Pl_trueobliqerr2 = table.Column<float>(type: "real", nullable: false),
                    Pl_trueobliqlim = table.Column<float>(type: "real", nullable: false),
                    St_refname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_spectype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_teff = table.Column<float>(type: "real", nullable: false),
                    St_tefferr1 = table.Column<float>(type: "real", nullable: false),
                    St_tefferr2 = table.Column<float>(type: "real", nullable: false),
                    St_tefflim = table.Column<float>(type: "real", nullable: false),
                    St_rad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_raderr1 = table.Column<float>(type: "real", nullable: false),
                    St_raderr2 = table.Column<float>(type: "real", nullable: false),
                    St_radlim = table.Column<float>(type: "real", nullable: false),
                    St_mass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_masserr1 = table.Column<float>(type: "real", nullable: false),
                    St_masserr2 = table.Column<float>(type: "real", nullable: false),
                    St_masslim = table.Column<float>(type: "real", nullable: false),
                    St_met = table.Column<float>(type: "real", nullable: false),
                    St_meterr1 = table.Column<float>(type: "real", nullable: false),
                    St_meterr2 = table.Column<float>(type: "real", nullable: false),
                    St_metlim = table.Column<float>(type: "real", nullable: false),
                    St_metratio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_lum = table.Column<float>(type: "real", nullable: false),
                    St_lumerr1 = table.Column<float>(type: "real", nullable: false),
                    St_lumerr2 = table.Column<float>(type: "real", nullable: false),
                    St_lumlim = table.Column<float>(type: "real", nullable: false),
                    St_logg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_loggerr1 = table.Column<float>(type: "real", nullable: false),
                    St_loggerr2 = table.Column<float>(type: "real", nullable: false),
                    St_logglim = table.Column<float>(type: "real", nullable: false),
                    St_age = table.Column<float>(type: "real", nullable: false),
                    St_ageerr1 = table.Column<float>(type: "real", nullable: false),
                    St_ageerr2 = table.Column<float>(type: "real", nullable: false),
                    St_agelim = table.Column<float>(type: "real", nullable: false),
                    St_dens = table.Column<float>(type: "real", nullable: false),
                    St_denserr1 = table.Column<float>(type: "real", nullable: false),
                    St_denserr2 = table.Column<float>(type: "real", nullable: false),
                    St_denslim = table.Column<float>(type: "real", nullable: false),
                    St_vsin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_vsinerr1 = table.Column<float>(type: "real", nullable: false),
                    St_vsinerr2 = table.Column<float>(type: "real", nullable: false),
                    St_vsinlim = table.Column<float>(type: "real", nullable: false),
                    St_rotp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_rotperr1 = table.Column<float>(type: "real", nullable: false),
                    St_rotperr2 = table.Column<float>(type: "real", nullable: false),
                    St_rotplim = table.Column<float>(type: "real", nullable: false),
                    St_radv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    St_radverr1 = table.Column<float>(type: "real", nullable: false),
                    St_radverr2 = table.Column<float>(type: "real", nullable: false),
                    St_radvlim = table.Column<float>(type: "real", nullable: false),
                    Sy_refname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rastr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ra = table.Column<float>(type: "real", nullable: false),
                    Decstr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dec = table.Column<float>(type: "real", nullable: false),
                    Glat = table.Column<float>(type: "real", nullable: false),
                    Glon = table.Column<float>(type: "real", nullable: false),
                    Elat = table.Column<float>(type: "real", nullable: false),
                    Elon = table.Column<float>(type: "real", nullable: false),
                    Sy_pm = table.Column<float>(type: "real", nullable: false),
                    Sy_pmerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_pmerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_pmra = table.Column<float>(type: "real", nullable: false),
                    Sy_pmraerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_pmraerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_pmdec = table.Column<float>(type: "real", nullable: false),
                    Sy_pmdecerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_pmdecerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_dist = table.Column<float>(type: "real", nullable: false),
                    Sy_disterr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_disterr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_plx = table.Column<float>(type: "real", nullable: false),
                    Sy_plxerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_plxerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_bmag = table.Column<float>(type: "real", nullable: false),
                    Sy_bmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_bmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_vmag = table.Column<float>(type: "real", nullable: false),
                    Sy_vmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_vmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_jmag = table.Column<float>(type: "real", nullable: false),
                    Sy_jmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_jmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_hmag = table.Column<float>(type: "real", nullable: false),
                    Sy_hmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_hmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_kmag = table.Column<float>(type: "real", nullable: false),
                    Sy_kmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_kmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_umag = table.Column<float>(type: "real", nullable: false),
                    Sy_umagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_umagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_gmag = table.Column<float>(type: "real", nullable: false),
                    Sy_gmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_gmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_rmag = table.Column<float>(type: "real", nullable: false),
                    Sy_rmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_rmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_imag = table.Column<float>(type: "real", nullable: false),
                    Sy_imagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_imagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_zmag = table.Column<float>(type: "real", nullable: false),
                    Sy_zmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_zmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_w1mag = table.Column<float>(type: "real", nullable: false),
                    Sy_w1magerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_w1magerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_w2mag = table.Column<float>(type: "real", nullable: false),
                    Sy_w2magerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_w2magerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_w3mag = table.Column<float>(type: "real", nullable: false),
                    Sy_w3magerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_w3magerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_w4mag = table.Column<float>(type: "real", nullable: false),
                    Sy_w4magerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_w4magerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_gaiamag = table.Column<float>(type: "real", nullable: false),
                    Sy_gaiamagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_gaiamagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_icmag = table.Column<float>(type: "real", nullable: false),
                    Sy_icmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_icmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_tmag = table.Column<float>(type: "real", nullable: false),
                    Sy_tmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_tmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Sy_kepmag = table.Column<float>(type: "real", nullable: false),
                    Sy_kepmagerr1 = table.Column<float>(type: "real", nullable: false),
                    Sy_kepmagerr2 = table.Column<float>(type: "real", nullable: false),
                    Rowupdate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Pl_pubdate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Releasedate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Pl_nnotes = table.Column<int>(type: "int", nullable: false),
                    St_nphot = table.Column<int>(type: "int", nullable: false),
                    St_nrvc = table.Column<int>(type: "int", nullable: false),
                    St_nspec = table.Column<int>(type: "int", nullable: false),
                    Pl_nespec = table.Column<int>(type: "int", nullable: false),
                    Pl_ntranspec = table.Column<int>(type: "int", nullable: false),
                    Pl_ndispec = table.Column<int>(type: "int", nullable: false),
                    PageNumber = table.Column<int>(type: "int", nullable: true),
                    PageCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NASAExoplanetCatalog", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "NGCICOpendatasoft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NGC_IC = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Name = table.Column<int>(type: "int", nullable: true),
                    SubObject = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Messier = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Name_UK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Other_names = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    NGC = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    IC = table.Column<string>(type: "nvarchar(23)", maxLength: 23, nullable: true),
                    ObjectTypeAbrev = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: true),
                    ObjectType = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: true),
                    Object_type = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Source_Type = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    RA = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Right_ascension = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, computedColumnSql: "\r\n                ISNULL(CAST([Right_ascension_H] AS varchar(10)), '') + 'h ' + \r\n                ISNULL(CAST([Right_ascension_M] AS varchar(10)), '') + 'm ' + \r\n                ISNULL(CAST([Right_ascension_S] AS varchar(10)), '') + 's'", stored: true),
                    Right_ascension_H = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Right_ascension_M = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Right_ascension_S = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    DEC = table.Column<string>(type: "nvarchar(31)", maxLength: 31, nullable: true),
                    Declination = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, computedColumnSql: "\r\n                ISNULL(CAST([Declination_D] AS varchar(10)), '') + '° ' + \r\n                ISNULL(CAST([Declination_M] AS varchar(10)), '') + ''' ' + \r\n                ISNULL(CAST([Declination_S] AS varchar(10)), '') + '\"'", stored: true),
                    NS = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Declination_D = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Declination_M = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Declination_S = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Constellation = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: true),
                    MajorAxis = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    MinorAxis = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    PositionAngle = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    App_Mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    App_Mag_Flag = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    b_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    v_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    j_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    h_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    k_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Surface_Brigthness = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Hubble_OnlyGalaxies = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Cstar_UMag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Cstar_BMag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Cstar_VMag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Cstar_Names = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: true),
                    CommonNames = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    NedNotes = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    OpenngcNotes = table.Column<string>(type: "nvarchar(330)", maxLength: 330, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageNumber = table.Column<int>(type: "int", nullable: true),
                    PageCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGCICOpendatasoft", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NGCICOpendatasoft_Extension",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NGC_IC = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Name = table.Column<int>(type: "int", nullable: true),
                    SubObject = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Messier = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Name_UK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Other_names = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    NGC = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    IC = table.Column<string>(type: "nvarchar(23)", maxLength: 23, nullable: true),
                    ObjectTypeAbrev = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: true),
                    ObjectType = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: true),
                    Object_type = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Source_Type = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    RA = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Right_ascension = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, computedColumnSql: "\r\n                ISNULL(CAST([Right_ascension_H] AS varchar(10)), '') + 'h ' + \r\n                ISNULL(CAST([Right_ascension_M] AS varchar(10)), '') + 'm ' + \r\n                ISNULL(CAST([Right_ascension_S] AS varchar(10)), '') + 's'", stored: true),
                    Right_ascension_H = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Right_ascension_M = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Right_ascension_S = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    DEC = table.Column<string>(type: "nvarchar(31)", maxLength: 31, nullable: true),
                    Declination = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, computedColumnSql: "\r\n                ISNULL(CAST([Declination_D] AS varchar(10)), '') + '° ' + \r\n                ISNULL(CAST([Declination_M] AS varchar(10)), '') + ''' ' + \r\n                ISNULL(CAST([Declination_S] AS varchar(10)), '') + '\"'", stored: true),
                    NS = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Declination_D = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Declination_M = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Declination_S = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Constellation = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: true),
                    MajorAxis = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    MinorAxis = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    PositionAngle = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    App_Mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    App_Mag_Flag = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    b_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    v_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    j_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    h_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    k_mag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Surface_Brigthness = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Hubble_OnlyGalaxies = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Cstar_UMag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Cstar_BMag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Cstar_VMag = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Cstar_Names = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: true),
                    CommonNames = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    NedNotes = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    OpenngcNotes = table.Column<string>(type: "nvarchar(330)", maxLength: 330, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGCICOpendatasoft_Extension", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NLogApplicationCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Logged = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "varchar(10)", nullable: true),
                    Ip = table.Column<string>(type: "varchar(50)", nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Logger = table.Column<string>(type: "varchar(300)", nullable: true),
                    Controller = table.Column<string>(type: "varchar(100)", nullable: true),
                    Action = table.Column<string>(type: "varchar(50)", nullable: true),
                    Method = table.Column<string>(type: "varchar(300)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Meaning = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestConnectionForNLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Logger = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestConnectionForNLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UNIQUE_NGC_IC_Name",
                table: "NGCICOpendatasoft",
                columns: new[] { "NGC_IC", "Name" },
                unique: true,
                filter: "[NGC_IC] IS NOT NULL AND [Name] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CollinderCatalog");

            migrationBuilder.DropTable(
                name: "Constellation");

            migrationBuilder.DropTable(
                name: "DateTable");

            migrationBuilder.DropTable(
                name: "LogProcFunc");

            migrationBuilder.DropTable(
                name: "NameObject");

            migrationBuilder.DropTable(
                name: "NASAExoplanetCatalog");

            migrationBuilder.DropTable(
                name: "NGCICOpendatasoft");

            migrationBuilder.DropTable(
                name: "NGCICOpendatasoft_Extension");

            migrationBuilder.DropTable(
                name: "NLogApplicationCode");

            migrationBuilder.DropTable(
                name: "SourceType");

            migrationBuilder.DropTable(
                name: "TestConnectionForNLog");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
