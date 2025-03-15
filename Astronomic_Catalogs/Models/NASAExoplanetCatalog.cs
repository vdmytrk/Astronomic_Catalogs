namespace Astronomic_Catalogs.Models;

public class NASAExoplanetCatalog
{
    public int RowId { get; set; }
    public string PlName { get; set; } = string.Empty; // For clear code without squiggle.
    public string Hostname { get; set; } = string.Empty;
    public string PlLetter { get; set; } = string.Empty;
    public string HdName { get; set; } = string.Empty;
    public string HipName { get; set; } = string.Empty;
    public string TicId { get; set; } = string.Empty;
    public string GaiaId { get; set; } = string.Empty;
    public int DefaultFlag { get; set; }
    public int SySnum { get; set; }
    public int SyPnum { get; set; }
    public int SyMnum { get; set; }
    public int CbFlag { get; set; }
    public string DiscoveryMethod { get; set; } = string.Empty;
    public int DiscYear { get; set; }
    public string DiscRefName { get; set; } = string.Empty;
    public DateTime DiscPubDate { get; set; } = DateTime.MinValue;
    public string DiscLocale { get; set; } = string.Empty;
    public string DiscFacility { get; set; } = string.Empty;
    public string DiscTelescope { get; set; } = string.Empty;
    public string DiscInstrument { get; set; } = string.Empty;
    public int RvFlag { get; set; }
    public int PulFlag { get; set; }
    public int PtvFlag { get; set; }
    public int TranFlag { get; set; }
    public int AstFlag { get; set; }
    public int ObmFlag { get; set; }
    public int MicroFlag { get; set; }
    public int EtvFlag { get; set; }
    public int ImaFlag { get; set; }
    public int DkinFlag { get; set; }
    public string SolType { get; set; } = string.Empty;
    public int PlControvFlag { get; set; }
    public string PlRefName { get; set; } = string.Empty;
    public float PlOrbPer { get; set; }
    public float PlOrbPerErr1 { get; set; }
    public float PlOrbPerErr2 { get; set; }
    public int PlOrbPerLim { get; set; }
    public float PlOrbSmax { get; set; }
    public float PlOrbSmaxErr1 { get; set; }
    public float PlOrbSmaxErr2 { get; set; }
    public int PlOrbSmaxLim { get; set; }
    public float PlRade { get; set; }
    public float PlRadeErr1 { get; set; }
    public float PlRadeErr2 { get; set; }
    public int PlRadeLim { get; set; }
    public float PlRadJ { get; set; }
    public float PlRadJErr1 { get; set; }
    public float PlRadJErr2 { get; set; }
    public int PlRadJLim { get; set; }
    public float PlMasse { get; set; }
    public float PlMasseErr1 { get; set; }
    public float PlMasseErr2 { get; set; }
    public int PlMasseLim { get; set; }

    // - 1 - Added for navigation.

    public float PlMassJ { get; set; }
    public float PlMassJErr1 { get; set; }
    public float PlMassJErr2 { get; set; }
    public int PlMassJLim { get; set; }
    public float PlMsiniE { get; set; }
    public float PlMsiniEErr1 { get; set; }
    public float PlMsiniEErr2 { get; set; }
    public int PlMsiniELim { get; set; }
    public float PlMsiniJ { get; set; }
    public float PlMsiniJErr1 { get; set; }
    public float PlMsiniJErr2 { get; set; }
    public int PlMsiniJLim { get; set; }
    public float PlCMasse { get; set; }
    public float PlCMasseErr1 { get; set; }
    public float PlCMasseErr2 { get; set; }
    public int PlCMasseLim { get; set; }
    public float PlCMassJ { get; set; }
    public float PlCMassJErr1 { get; set; }
    public float PlCMassJErr2 { get; set; }
    public int PlCMassJLim { get; set; }
    public float PlBmasse { get; set; }
    public float PlBmasseErr1 { get; set; }
    public float PlBmasseErr2 { get; set; }
    public int PlBmasseLim { get; set; }
    public float PlBmassJ { get; set; }
    public float PlBmassJErr1 { get; set; }
    public float PlBmassJErr2 { get; set; }
    public int PlBmassJLim { get; set; }
    public string PlBmassProv { get; set; } = string.Empty;
    public float PlDens { get; set; }
    public float PlDensErr1 { get; set; }
    public float PlDensErr2 { get; set; }
    public int PlDensLim { get; set; }
    public float PlOrbEccen { get; set; }
    public float PlOrbEccenErr1 { get; set; }
    public float PlOrbEccenErr2 { get; set; } 
	public int PlOrbeccenLim { get; set; } 
    public float PlInsol { get; set; }
    public float PlInsolErr1 { get; set; }
    public float PlInsolErr2 { get; set; }
    public int PlInsolLim { get; set; }
    public float PlEqt { get; set; }
    public float PlEqtErr1 { get; set; }
    public float PlEqtErr2 { get; set; }
    public int PlEqtLim { get; set; }
    public float PlOrbincl { get; set; }
    public float PlOrbinclErr1 { get; set; }
    public float PlOrbinclErr2 { get; set; } 
    public int PlOrbinclLim { get; set; }

    // - 2 - Added for navigation.

    public float PlTranmid { get; set; }
    public float PlTranmidErr1 { get; set; }
    public float PlTranmidErr2 { get; set; }
    public int PlTranmidLim { get; set; }
    public string PlTsystemref { get; set; } = string.Empty;
    public int TtvFlag { get; set; }
    public float PlImppar { get; set; }
    public float PlImpparErr1 { get; set; }
    public float PlImpparErr2 { get; set; }
    public int PlImpparLim { get; set; }
    public float PlTrandep { get; set; }
    public float PlTrandepErr1 { get; set; }
    public float PlTrandepErr2 { get; set; }
    public int PlTrandepLim { get; set; }
    public float PlTrandur { get; set; }
    public float PlTrandurErr1 { get; set; }
    public float PlTrandurErr2 { get; set; }
    public int PlTrandurLim { get; set; }
    public float PlRatdor { get; set; }
    public float PlRatdorErr1 { get; set; }
    public float PlRatdorErr2 { get; set; }
    public int PlRatdorLim { get; set; }
    public float PlRatror { get; set; }
    public float PlRatrorErr1 { get; set; }
    public float PlRatrorErr2 { get; set; }
    public int PlRatrorLim { get; set; }
    public float PlOccdep { get; set; }
    public float PlOccdepErr1 { get; set; }
    public float PlOccdepErr2 { get; set; }
    public int PlOccdepLim { get; set; }
    public float PlOrbtper { get; set; }
    public float PlOrbtperErr1 { get; set; }
    public float PlOrbtperErr2 { get; set; }
    public int PlOrbtperLim { get; set; }
    public float PlOrblper { get; set; }
    public float PlOrblperErr1 { get; set; }
    public float PlOrblperErr2 { get; set; }
    public int PlOrblperLim { get; set; }
    public float PlRvamp { get; set; }
    public float PlRvampErr1 { get; set; }
    public float PlRvampErr2 { get; set; }
    public int PlRvampLim { get; set; }
    public float PlProjobliq { get; set; }
    public float PlProjobliqErr1 { get; set; }
    public float PlProjobliqErr2 { get; set; }
    public int PlProjobliqLim { get; set; }
    public float PlTrueobliq { get; set; }
    public float PlTrueobliqErr1 { get; set; }
    public float PlTrueobliqErr2 { get; set; }
    public int PlTrueobliqLim { get; set; }

    // - 3 - Added for navigation.

    public string StRefname { get; set; } = string.Empty;
    public string StSpectype { get; set; } = string.Empty;
    public float StTeff { get; set; }
    public float StTeffErr1 { get; set; }
    public float StTeffErr2 { get; set; }
    public int StTeffLim { get; set; }
    public float StRad { get; set; }
    public float StRadErr1 { get; set; }
    public float StRadErr2 { get; set; }
    public int StRadLim { get; set; }
    public float StMass { get; set; }
    public float StMassErr1 { get; set; }
    public float StMassErr2 { get; set; }
    public int StMassLim { get; set; }
    public float StMet { get; set; }
    public float StMetErr1 { get; set; }
    public float StMetErr2 { get; set; }
    public int StMetLim { get; set; }
    public string StMetratio { get; set; } = string.Empty;
    public float StLum { get; set; }
    public float StLumErr1 { get; set; }
    public float StLumErr2 { get; set; }
    public int StLumLim { get; set; }
    public float StLogg { get; set; }
    public float StLoggErr1 { get; set; }
    public float StLoggErr2 { get; set; } 
    public int StLoggLim { get; set; }
    public float StAge { get; set; }
    public float StAgeErr1 { get; set; }
    public float StAgeErr2 { get; set; }
    public int StAgeLim { get; set; }
    public float StDens { get; set; }
    public float StDensErr1 { get; set; }
    public float StDensErr2 { get; set; }
    public int StDensLim { get; set; }
    public float StVsin { get; set; }
    public float StVsinErr1 { get; set; }
    public float StVsinErr2 { get; set; }
    public int StVsinLim { get; set; }
    public float StRotp { get; set; }
    public float StRotpErr1 { get; set; }
    public float StRotpErr2 { get; set; }
    public int StRotpLim { get; set; }
    public float StRadv { get; set; }
    public float StRadvErr1 { get; set; }
    public float StRadvErr2 { get; set; }
    public int StRadvLim { get; set; }
    public string SyRefName { get; set; } = string.Empty;
    public string Rastr { get; set; } = string.Empty;
    public float Ra { get; set; }
    public string Decstr { get; set; } = string.Empty;
    public float Dec { get; set; } 
    public float Glat { get; set; }
    public float Glon { get; set; }
    public float Elat { get; set; }
    public float Elon { get; set; }

    // - 4 - Added for navigation.

    public float SyPm { get; set; }
    public float SyPmErr1 { get; set; }
    public float SyPmErr2 { get; set; }
    public float SyPmRa { get; set; }
    public float SyPmRaErr1 { get; set; }
    public float SyPmRaErr2 { get; set; }
    public float SyPmDec { get; set; }
    public float SyPmDecErr1 { get; set; }
    public float SyPmDecErr2 { get; set; }
    public float SyDist { get; set; }
    public float SyDistErr1 { get; set; }
    public float SyDistErr2 { get; set; }
    public float SyPlx { get; set; }
    public float SyPlxErr1 { get; set; }
    public float SyPlxErr2 { get; set; }
    public float SyBmag { get; set; }
    public float SyBmagErr1 { get; set; }
    public float SyBmagErr2 { get; set; }
    public float SyVmag { get; set; }
    public float SyVmagErr1 { get; set; }
    public float SyVmagErr2 { get; set; }
    public float SyJmag { get; set; }
    public float SyJmagErr1 { get; set; }
    public float SyJmagErr2 { get; set; }
    public float SyHmag { get; set; }
    public float SyHmagErr1 { get; set; }
    public float SyHmagErr2 { get; set; }
    public float SyKmag { get; set; }
    public float SyKmagErr1 { get; set; }
    public float SyKmagErr2 { get; set; }
    public float SyUmag { get; set; }
    public float SyUmagErr1 { get; set; }
    public float SyUmagErr2 { get; set; }
    public float SyGmag { get; set; }
    public float SyGmagErr1 { get; set; }
    public float SyGmagErr2 { get; set; }
    public float SyRmag { get; set; }
    public float SyRmagErr1 { get; set; }
    public float SyRmagErr2 { get; set; }
    public float SyImag { get; set; }
    public float SyImagErr1 { get; set; }
    public float SyImagErr2 { get; set; }
    public float SyZmag { get; set; }
    public float SyZmagErr1 { get; set; }
    public float SyZmagErr2 { get; set; }

    // - 5 - Added for navigation.

    public float SyW1mag { get; set; }
    public float SyW1magErr1 { get; set; }
    public float SyW1magErr2 { get; set; }
    public float SyW2mag { get; set; }
    public float SyW2magErr1 { get; set; }
    public float SyW2magErr2 { get; set; }
    public float SyW3mag { get; set; }
    public float SyW3magErr1 { get; set; }
    public float SyW3magErr2 { get; set; }
    public float SyW4mag { get; set; }
    public float SyW4magErr1 { get; set; }
    public float SyW4magErr2 { get; set; }
    public float SyGaiaMag { get; set; }  
    public float SyGaiamagerr1 { get; set; }
    public float SyGaiamagerr2 { get; set; }
    public float SyIcmag { get; set; }
    public float SyIcmagerr1 { get; set; }
    public float SyIcmagerr2 { get; set; }
    public float SyTmag { get; set; }
    public float SyTmagerr1 { get; set; }
    public float SyTmagerr2 { get; set; }
    public float SyKepmag { get; set; }
    public float SyKepmagerr1 { get; set; }
    public float SyKepmagerr2 { get; set; }
    public DateTime Rowupdate { get; set; } = DateTime.MinValue;
    public DateTime PlPubdate { get; set; } = DateTime.MinValue;
    public DateTime Releasedate { get; set; } = DateTime.MinValue;
    public int PlNnotes { get; set; }
    public int StNphot { get; set; }
    public int StNrvc { get; set; }
    public int StNspec { get; set; }
    public int PlNespec { get; set; }
    public int PlNtranspec { get; set; }
    public int PlNdispec { get; set; }


    public int? PageNumber { get; set; }
    public int? PageCount { get; set; }
}
