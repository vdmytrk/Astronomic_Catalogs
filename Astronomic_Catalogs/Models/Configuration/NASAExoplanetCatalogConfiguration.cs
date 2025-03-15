using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration;

public class NASAExoplanetCatalogConfiguration : IEntityTypeConfiguration<NASAExoplanetCatalog>
{
    public void Configure(EntityTypeBuilder<NASAExoplanetCatalog> builder)
    {
        builder.ToTable("NASAExoplanetCatalog");

        builder.HasKey(e => e.RowId);
        builder.Property(e => e.RowId)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.PlName).HasColumnName("Pl_name");
        builder.Property(e => e.PlLetter).HasColumnName("Pl_letter");
        builder.Property(e => e.HdName).HasColumnName("Hd_name");
        builder.Property(e => e.HipName).HasColumnName("Hip_name");
        builder.Property(e => e.TicId).HasColumnName("Tic_id");
        builder.Property(e => e.GaiaId).HasColumnName("Gaia_id");

        builder.Property(e => e.DefaultFlag).HasColumnName("Default_flag");
        builder.Property(e => e.SySnum).HasColumnName("Sy_snum");
        builder.Property(e => e.SyPnum).HasColumnName("Sy_pnum");
        builder.Property(e => e.SyMnum).HasColumnName("Sy_mnum");
        builder.Property(e => e.CbFlag).HasColumnName("Cb_flag");

        builder.Property(e => e.DiscoveryMethod).HasColumnName("Discoverymethod");
        builder.Property(e => e.DiscYear).HasColumnName("Disc_year");
        builder.Property(e => e.DiscRefName).HasColumnName("Disc_refname");
        builder.Property(e => e.DiscPubDate).HasColumnName("Disc_pubdate").HasDefaultValue(DateTime.MinValue);
        builder.Property(e => e.DiscLocale).HasColumnName("Disc_locale");
        builder.Property(e => e.DiscFacility).HasColumnName("Disc_facility");
        builder.Property(e => e.DiscTelescope).HasColumnName("Disc_telescope");
        builder.Property(e => e.DiscInstrument).HasColumnName("Disc_instrument");

        builder.Property(e => e.RvFlag).HasColumnName("Rv_flag");
        builder.Property(e => e.PulFlag).HasColumnName("Pul_flag");
        builder.Property(e => e.PtvFlag).HasColumnName("Ptv_flag");
        builder.Property(e => e.TranFlag).HasColumnName("Tran_flag");
        builder.Property(e => e.AstFlag).HasColumnName("Ast_flag");
        builder.Property(e => e.ObmFlag).HasColumnName("Obm_flag");
        builder.Property(e => e.MicroFlag).HasColumnName("Micro_flag");
        builder.Property(e => e.EtvFlag).HasColumnName("Etv_flag");
        builder.Property(e => e.ImaFlag).HasColumnName("Ima_flag");
        builder.Property(e => e.DkinFlag).HasColumnName("Dkin_flag");

        builder.Property(e => e.SolType).HasColumnName("Soltype");
        builder.Property(e => e.PlControvFlag).HasColumnName("Pl_controv_flag");
        builder.Property(e => e.PlRefName).HasColumnName("Pl_refname");

        builder.Property(e => e.PlOrbPer).HasColumnName("Pl_orbper");
        builder.Property(e => e.PlOrbPerErr1).HasColumnName("Pl_orbpererr1");
        builder.Property(e => e.PlOrbPerErr2).HasColumnName("Pl_orbpererr2");
        builder.Property(e => e.PlOrbPerLim).HasColumnName("Pl_orbperlim");

        builder.Property(e => e.PlOrbSmax).HasColumnName("Pl_orbsmax");
        builder.Property(e => e.PlOrbSmaxErr1).HasColumnName("Pl_orbsmaxerr1");
        builder.Property(e => e.PlOrbSmaxErr2).HasColumnName("Pl_orbsmaxerr2");
        builder.Property(e => e.PlOrbSmaxLim).HasColumnName("Pl_orbsmaxlim");

        builder.Property(e => e.PlRade).HasColumnName("Pl_rade");
        builder.Property(e => e.PlRadeErr1).HasColumnName("Pl_radeerr1");
        builder.Property(e => e.PlRadeErr2).HasColumnName("Pl_radeerr2");
        builder.Property(e => e.PlRadeLim).HasColumnName("Pl_radelim");

        builder.Property(e => e.PlRadJ).HasColumnName("Pl_radj");
        builder.Property(e => e.PlRadJErr1).HasColumnName("Pl_radjerr1");
        builder.Property(e => e.PlRadJErr2).HasColumnName("Pl_radjerr2");
        builder.Property(e => e.PlRadJLim).HasColumnName("Pl_radjlim");

        builder.Property(e => e.PlMasse).HasColumnName("Pl_masse");
        builder.Property(e => e.PlMasseErr1).HasColumnName("Pl_masseerr1");
        builder.Property(e => e.PlMasseErr2).HasColumnName("Pl_masseerr2");
        builder.Property(e => e.PlMasseLim).HasColumnName("Pl_masselim");

        // - 1

        builder.Property(e => e.PlMassJ).HasColumnName("Pl_massj");
        builder.Property(e => e.PlMassJErr1).HasColumnName("Pl_massjerr1");
        builder.Property(e => e.PlMassJErr2).HasColumnName("Pl_massjerr2");
        builder.Property(e => e.PlMassJLim).HasColumnName("Pl_massjlim");

        builder.Property(e => e.PlMsiniE).HasColumnName("Pl_msinie");
        builder.Property(e => e.PlMsiniEErr1).HasColumnName("Pl_msinieerr1");
        builder.Property(e => e.PlMsiniEErr2).HasColumnName("Pl_msinieerr2");
        builder.Property(e => e.PlMsiniELim).HasColumnName("Pl_msinielim");

        builder.Property(e => e.PlMsiniJ).HasColumnName("Pl_msinij");
        builder.Property(e => e.PlMsiniJErr1).HasColumnName("Pl_msinijerr1");
        builder.Property(e => e.PlMsiniJErr2).HasColumnName("Pl_msinijerr2");
        builder.Property(e => e.PlMsiniJLim).HasColumnName("Pl_msinijlim");

        builder.Property(e => e.PlCMasse).HasColumnName("Pl_cmasse");
        builder.Property(e => e.PlCMasseErr1).HasColumnName("Pl_cmasseerr1");
        builder.Property(e => e.PlCMasseErr2).HasColumnName("Pl_cmasseerr2");
        builder.Property(e => e.PlCMasseLim).HasColumnName("Pl_cmasselim");

        builder.Property(e => e.PlCMassJ).HasColumnName("Pl_cmassj");
        builder.Property(e => e.PlCMassJErr1).HasColumnName("Pl_cmassjerr1");
        builder.Property(e => e.PlCMassJErr2).HasColumnName("Pl_cmassjerr2");
        builder.Property(e => e.PlCMassJLim).HasColumnName("Pl_cmassjlim");

        builder.Property(e => e.PlBmasse).HasColumnName("Pl_bmasse");
        builder.Property(e => e.PlBmasseErr1).HasColumnName("Pl_bmasseerr1");
        builder.Property(e => e.PlBmasseErr2).HasColumnName("Pl_bmasseerr2");
        builder.Property(e => e.PlBmasseLim).HasColumnName("Pl_bmasselim");

        builder.Property(e => e.PlBmassJ).HasColumnName("Pl_bmassj");
        builder.Property(e => e.PlBmassJErr1).HasColumnName("Pl_bmassjerr1");
        builder.Property(e => e.PlBmassJErr2).HasColumnName("Pl_bmassjerr2");
        builder.Property(e => e.PlBmassJLim).HasColumnName("Pl_bmassjlim");

        builder.Property(e => e.PlBmassProv).HasColumnName("Pl_bmassprov");

        builder.Property(e => e.PlDens).HasColumnName("Pl_dens");
        builder.Property(e => e.PlDensErr1).HasColumnName("Pl_denserr1");
        builder.Property(e => e.PlDensErr2).HasColumnName("Pl_denserr2");
        builder.Property(e => e.PlDensLim).HasColumnName("Pl_denslim");

        builder.Property(e => e.PlOrbEccen).HasColumnName("Pl_orbeccen");
        builder.Property(e => e.PlOrbEccenErr1).HasColumnName("Pl_orbeccenerr1");
        builder.Property(e => e.PlOrbEccenErr2).HasColumnName("Pl_orbeccenerr2");
        builder.Property(e => e.PlOrbeccenLim).HasColumnName("Pl_orbeccenlim");

        builder.Property(e => e.PlInsol).HasColumnName("Pl_insol");
        builder.Property(e => e.PlInsolErr1).HasColumnName("Pl_insolerr1");
        builder.Property(e => e.PlInsolErr2).HasColumnName("Pl_insolerr2");
        builder.Property(e => e.PlInsolLim).HasColumnName("Pl_insollim");

        builder.Property(e => e.PlEqt).HasColumnName("Pl_eqt");
        builder.Property(e => e.PlEqtErr1).HasColumnName("Pl_eqterr1");
        builder.Property(e => e.PlEqtErr2).HasColumnName("Pl_eqterr2");
        builder.Property(e => e.PlEqtLim).HasColumnName("Pl_eqtlim");

        builder.Property(e => e.PlOrbincl).HasColumnName("Pl_orbincl");
        builder.Property(e => e.PlOrbinclErr1).HasColumnName("Pl_orbinclerr1");
        builder.Property(e => e.PlOrbinclErr2).HasColumnName("Pl_orbinclerr2");
        builder.Property(e => e.PlOrbinclLim).HasColumnName("Pl_orbincllim");

        // - 2

        builder.Property(e => e.PlTranmid).HasColumnName("Pl_tranmid");
        builder.Property(e => e.PlTranmidErr1).HasColumnName("Pl_tranmiderr1");
        builder.Property(e => e.PlTranmidErr2).HasColumnName("Pl_tranmiderr2");
        builder.Property(e => e.PlTranmidLim).HasColumnName("Pl_tranmidlim");

        builder.Property(e => e.PlTsystemref).HasColumnName("Pl_tsystemref");
        builder.Property(e => e.TtvFlag).HasColumnName("Ttv_flag");

        builder.Property(e => e.PlImppar).HasColumnName("Pl_imppar");
        builder.Property(e => e.PlImpparErr1).HasColumnName("Pl_impparerr1");
        builder.Property(e => e.PlImpparErr2).HasColumnName("Pl_impparerr2");
        builder.Property(e => e.PlImpparLim).HasColumnName("Pl_impparlim");

        builder.Property(e => e.PlTrandep).HasColumnName("Pl_trandep");
        builder.Property(e => e.PlTrandepErr1).HasColumnName("Pl_trandeperr1");
        builder.Property(e => e.PlTrandepErr2).HasColumnName("Pl_trandeperr2");
        builder.Property(e => e.PlTrandepLim).HasColumnName("Pl_trandeplim");

        builder.Property(e => e.PlTrandur).HasColumnName("Pl_trandur");
        builder.Property(e => e.PlTrandurErr1).HasColumnName("Pl_trandurerr1");
        builder.Property(e => e.PlTrandurErr2).HasColumnName("Pl_trandurerr2");
        builder.Property(e => e.PlTrandurLim).HasColumnName("Pl_trandurlim");

        builder.Property(e => e.PlRatdor).HasColumnName("Pl_ratdor");
        builder.Property(e => e.PlRatdorErr1).HasColumnName("Pl_ratdorerr1");
        builder.Property(e => e.PlRatdorErr2).HasColumnName("Pl_ratdorerr2");
        builder.Property(e => e.PlRatdorLim).HasColumnName("Pl_ratdorlim");

        builder.Property(e => e.PlRatror).HasColumnName("Pl_ratror");
        builder.Property(e => e.PlRatrorErr1).HasColumnName("Pl_ratrorerr1");
        builder.Property(e => e.PlRatrorErr2).HasColumnName("Pl_ratrorerr2");
        builder.Property(e => e.PlRatrorLim).HasColumnName("Pl_ratrorlim");

        builder.Property(e => e.PlOccdep).HasColumnName("Pl_occdep");
        builder.Property(e => e.PlOccdepErr1).HasColumnName("Pl_occdeperr1");
        builder.Property(e => e.PlOccdepErr2).HasColumnName("Pl_occdeperr2");
        builder.Property(e => e.PlOccdepLim).HasColumnName("Pl_occdeplim");

        builder.Property(e => e.PlOrbtper).HasColumnName("Pl_orbtper");
        builder.Property(e => e.PlOrbtperErr1).HasColumnName("Pl_orbtpererr1");
        builder.Property(e => e.PlOrbtperErr2).HasColumnName("Pl_orbtpererr2");
        builder.Property(e => e.PlOrbtperLim).HasColumnName("Pl_orbtperlim");

        builder.Property(e => e.PlOrblper).HasColumnName("Pl_orblper");
        builder.Property(e => e.PlOrblperErr1).HasColumnName("Pl_orblpererr1");
        builder.Property(e => e.PlOrblperErr2).HasColumnName("Pl_orblpererr2");
        builder.Property(e => e.PlOrblperLim).HasColumnName("Pl_orblperlim");

        builder.Property(e => e.PlRvamp).HasColumnName("Pl_rvamp");
        builder.Property(e => e.PlRvampErr1).HasColumnName("Pl_rvamperr1");
        builder.Property(e => e.PlRvampErr2).HasColumnName("Pl_rvamperr2");
        builder.Property(e => e.PlRvampLim).HasColumnName("Pl_rvamplim");

        builder.Property(e => e.PlProjobliq).HasColumnName("Pl_projobliq");
        builder.Property(e => e.PlProjobliqErr1).HasColumnName("Pl_projobliqerr1");
        builder.Property(e => e.PlProjobliqErr2).HasColumnName("Pl_projobliqerr2");
        builder.Property(e => e.PlProjobliqLim).HasColumnName("Pl_projobliqlim");

        builder.Property(e => e.PlTrueobliq).HasColumnName("Pl_trueobliq");
        builder.Property(e => e.PlTrueobliqErr1).HasColumnName("Pl_trueobliqerr1");
        builder.Property(e => e.PlTrueobliqErr2).HasColumnName("Pl_trueobliqerr2");
        builder.Property(e => e.PlTrueobliqLim).HasColumnName("Pl_trueobliqlim");


        // - 3

        builder.Property(e => e.StRefname).HasColumnName("St_refname");
        builder.Property(e => e.StSpectype).HasColumnName("St_spectype");

        builder.Property(e => e.StTeff).HasColumnName("St_teff");
        builder.Property(e => e.StTeffErr1).HasColumnName("St_tefferr1");
        builder.Property(e => e.StTeffErr2).HasColumnName("St_tefferr2");
        builder.Property(e => e.StTeffLim).HasColumnName("St_tefflim");

        builder.Property(e => e.StRad).HasColumnName("St_rad");
        builder.Property(e => e.StRadErr1).HasColumnName("St_raderr1");
        builder.Property(e => e.StRadErr2).HasColumnName("St_raderr2");
        builder.Property(e => e.StRadLim).HasColumnName("St_radlim");

        builder.Property(e => e.StMass).HasColumnName("St_mass");
        builder.Property(e => e.StMassErr1).HasColumnName("St_masserr1");
        builder.Property(e => e.StMassErr2).HasColumnName("St_masserr2");
        builder.Property(e => e.StMassLim).HasColumnName("St_masslim");

        builder.Property(e => e.StMet).HasColumnName("St_met");
        builder.Property(e => e.StMetErr1).HasColumnName("St_meterr1");
        builder.Property(e => e.StMetErr2).HasColumnName("St_meterr2");
        builder.Property(e => e.StMetLim).HasColumnName("St_metlim");

        builder.Property(e => e.StMetratio).HasColumnName("St_metratio");

        builder.Property(e => e.StLum).HasColumnName("St_lum");
        builder.Property(e => e.StLumErr1).HasColumnName("St_lumerr1");
        builder.Property(e => e.StLumErr2).HasColumnName("St_lumerr2");
        builder.Property(e => e.StLumLim).HasColumnName("St_lumlim");

        builder.Property(e => e.StLogg).HasColumnName("St_logg");
        builder.Property(e => e.StLoggErr1).HasColumnName("St_loggerr1");
        builder.Property(e => e.StLoggErr2).HasColumnName("St_loggerr2");
        builder.Property(e => e.StLoggLim).HasColumnName("St_logglim");

        builder.Property(e => e.StAge).HasColumnName("St_age");
        builder.Property(e => e.StAgeErr1).HasColumnName("St_ageerr1");
        builder.Property(e => e.StAgeErr2).HasColumnName("St_ageerr2");
        builder.Property(e => e.StAgeLim).HasColumnName("St_agelim");

        builder.Property(e => e.StDens).HasColumnName("St_dens");
        builder.Property(e => e.StDensErr1).HasColumnName("St_denserr1");
        builder.Property(e => e.StDensErr2).HasColumnName("St_denserr2");
        builder.Property(e => e.StDensLim).HasColumnName("St_denslim");

        builder.Property(e => e.StVsin).HasColumnName("St_vsin");
        builder.Property(e => e.StVsinErr1).HasColumnName("St_vsinerr1");
        builder.Property(e => e.StVsinErr2).HasColumnName("St_vsinerr2");
        builder.Property(e => e.StVsinLim).HasColumnName("St_vsinlim");

        builder.Property(e => e.StRotp).HasColumnName("St_rotp");
        builder.Property(e => e.StRotpErr1).HasColumnName("St_rotperr1");
        builder.Property(e => e.StRotpErr2).HasColumnName("St_rotperr2");
        builder.Property(e => e.StRotpLim).HasColumnName("St_rotplim");

        builder.Property(e => e.StRadv).HasColumnName("St_radv");
        builder.Property(e => e.StRadvErr1).HasColumnName("St_radverr1");
        builder.Property(e => e.StRadvErr2).HasColumnName("St_radverr2");
        builder.Property(e => e.StRadvLim).HasColumnName("St_radvlim");

        builder.Property(e => e.SyRefName).HasColumnName("Sy_refname");
        builder.Property(e => e.Rastr).HasColumnName("Rastr");
        builder.Property(e => e.Ra).HasColumnName("Ra");
        builder.Property(e => e.Decstr).HasColumnName("Decstr");
        builder.Property(e => e.Dec).HasColumnName("Dec");

        // - 4

        builder.Property(e => e.SyPm).HasColumnName("Sy_pm");
        builder.Property(e => e.SyPmErr1).HasColumnName("Sy_pmerr1");
        builder.Property(e => e.SyPmErr2).HasColumnName("Sy_pmerr2");

        builder.Property(e => e.SyPmRa).HasColumnName("Sy_pmra");
        builder.Property(e => e.SyPmRaErr1).HasColumnName("Sy_pmraerr1");
        builder.Property(e => e.SyPmRaErr2).HasColumnName("Sy_pmraerr2");

        builder.Property(e => e.SyPmDec).HasColumnName("Sy_pmdec");
        builder.Property(e => e.SyPmDecErr1).HasColumnName("Sy_pmdecerr1");
        builder.Property(e => e.SyPmDecErr2).HasColumnName("Sy_pmdecerr2");

        builder.Property(e => e.SyDist).HasColumnName("Sy_dist");
        builder.Property(e => e.SyDistErr1).HasColumnName("Sy_disterr1");
        builder.Property(e => e.SyDistErr2).HasColumnName("Sy_disterr2");

        builder.Property(e => e.SyPlx).HasColumnName("Sy_plx");
        builder.Property(e => e.SyPlxErr1).HasColumnName("Sy_plxerr1");
        builder.Property(e => e.SyPlxErr2).HasColumnName("Sy_plxerr2");

        builder.Property(e => e.SyBmag).HasColumnName("Sy_bmag");
        builder.Property(e => e.SyBmagErr1).HasColumnName("Sy_bmagerr1");
        builder.Property(e => e.SyBmagErr2).HasColumnName("Sy_bmagerr2");

        builder.Property(e => e.SyVmag).HasColumnName("Sy_vmag");
        builder.Property(e => e.SyVmagErr1).HasColumnName("Sy_vmagerr1");
        builder.Property(e => e.SyVmagErr2).HasColumnName("Sy_vmagerr2");

        builder.Property(e => e.SyJmag).HasColumnName("Sy_jmag");
        builder.Property(e => e.SyJmagErr1).HasColumnName("Sy_jmagerr1");
        builder.Property(e => e.SyJmagErr2).HasColumnName("Sy_jmagerr2");

        builder.Property(e => e.SyHmag).HasColumnName("Sy_hmag");
        builder.Property(e => e.SyHmagErr1).HasColumnName("Sy_hmagerr1");
        builder.Property(e => e.SyHmagErr2).HasColumnName("Sy_hmagerr2");

        builder.Property(e => e.SyKmag).HasColumnName("Sy_kmag");
        builder.Property(e => e.SyKmagErr1).HasColumnName("Sy_kmagerr1");
        builder.Property(e => e.SyKmagErr2).HasColumnName("Sy_kmagerr2");

        builder.Property(e => e.SyUmag).HasColumnName("Sy_umag");
        builder.Property(e => e.SyUmagErr1).HasColumnName("Sy_umagerr1");
        builder.Property(e => e.SyUmagErr2).HasColumnName("Sy_umagerr2");

        builder.Property(e => e.SyGmag).HasColumnName("Sy_gmag");
        builder.Property(e => e.SyGmagErr1).HasColumnName("Sy_gmagerr1");
        builder.Property(e => e.SyGmagErr2).HasColumnName("Sy_gmagerr2");

        builder.Property(e => e.SyRmag).HasColumnName("Sy_rmag");
        builder.Property(e => e.SyRmagErr1).HasColumnName("Sy_rmagerr1");
        builder.Property(e => e.SyRmagErr2).HasColumnName("Sy_rmagerr2");

        builder.Property(e => e.SyImag).HasColumnName("Sy_imag");
        builder.Property(e => e.SyImagErr1).HasColumnName("Sy_imagerr1");
        builder.Property(e => e.SyImagErr2).HasColumnName("Sy_imagerr2");

        builder.Property(e => e.SyZmag).HasColumnName("Sy_zmag");
        builder.Property(e => e.SyZmagErr1).HasColumnName("Sy_zmagerr1");
        builder.Property(e => e.SyZmagErr2).HasColumnName("Sy_zmagerr2");

        // - 5

        builder.Property(e => e.SyW1mag).HasColumnName("Sy_w1mag");
        builder.Property(e => e.SyW1magErr1).HasColumnName("Sy_w1magerr1");
        builder.Property(e => e.SyW1magErr2).HasColumnName("Sy_w1magerr2");

        builder.Property(e => e.SyW2mag).HasColumnName("Sy_w2mag");
        builder.Property(e => e.SyW2magErr1).HasColumnName("Sy_w2magerr1");
        builder.Property(e => e.SyW2magErr2).HasColumnName("Sy_w2magerr2");

        builder.Property(e => e.SyW3mag).HasColumnName("Sy_w3mag");
        builder.Property(e => e.SyW3magErr1).HasColumnName("Sy_w3magerr1");
        builder.Property(e => e.SyW3magErr2).HasColumnName("Sy_w3magerr2");

        builder.Property(e => e.SyW4mag).HasColumnName("Sy_w4mag");
        builder.Property(e => e.SyW4magErr1).HasColumnName("Sy_w4magerr1");
        builder.Property(e => e.SyW4magErr2).HasColumnName("Sy_w4magerr2");

        builder.Property(e => e.SyGaiaMag).HasColumnName("Sy_gaiamag");
        builder.Property(e => e.SyGaiamagerr1).HasColumnName("Sy_gaiamagerr1");
        builder.Property(e => e.SyGaiamagerr2).HasColumnName("Sy_gaiamagerr2");

        builder.Property(e => e.SyIcmag).HasColumnName("Sy_icmag");
        builder.Property(e => e.SyIcmagerr1).HasColumnName("Sy_icmagerr1");
        builder.Property(e => e.SyIcmagerr2).HasColumnName("Sy_icmagerr2");

        builder.Property(e => e.SyTmag).HasColumnName("Sy_tmag");
        builder.Property(e => e.SyTmagerr1).HasColumnName("Sy_tmagerr1");
        builder.Property(e => e.SyTmagerr2).HasColumnName("Sy_tmagerr2");

        builder.Property(e => e.SyKepmag).HasColumnName("Sy_kepmag");
        builder.Property(e => e.SyKepmagerr1).HasColumnName("Sy_kepmagerr1");
        builder.Property(e => e.SyKepmagerr2).HasColumnName("Sy_kepmagerr2");

        builder.Property(e => e.Rowupdate).HasColumnName("Rowupdate").HasDefaultValue(DateTime.MinValue);
        builder.Property(e => e.PlPubdate).HasColumnName("Pl_pubdate").HasDefaultValue(DateTime.MinValue);
        builder.Property(e => e.Releasedate).HasColumnName("Releasedate").HasDefaultValue(DateTime.MinValue);
        builder.Property(e => e.PlNnotes).HasColumnName("Pl_nnotes");
        builder.Property(e => e.StNphot).HasColumnName("St_nphot");
        builder.Property(e => e.StNrvc).HasColumnName("St_nrvc");
        builder.Property(e => e.StNspec).HasColumnName("St_nspec");
        builder.Property(e => e.PlNespec).HasColumnName("Pl_nespec");
        builder.Property(e => e.PlNtranspec).HasColumnName("Pl_ntranspec");
        builder.Property(e => e.PlNdispec).HasColumnName("Pl_ndispec");

        builder.Property(e => e.PageNumber).IsRequired(false);
        builder.Property(e => e.PageCount).IsRequired(false);
    }
}