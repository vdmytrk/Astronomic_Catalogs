using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Planetology.Controllers;

[Area("Planetology")]
public class PlanetsCatalogController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IExcelImport _excelImportService;
    private readonly IImportCancellationService _importCancellationService;
    private readonly IPlanetFilterService _filterService;
    private readonly IDistinctSelectListService _distinctSelectListService;

    public PlanetsCatalogController(
        ApplicationDbContext context,
        IExcelImport excelImport_OpenXml,
        IImportCancellationService importCancellationService,
        IPlanetFilterService filterService,
        IDistinctSelectListService distinctSelectListService
        )
    {
        _context = context;
        _excelImportService = excelImport_OpenXml;
        _importCancellationService = importCancellationService;
        _filterService = filterService;
        _distinctSelectListService = distinctSelectListService;
    }

    // GET: Planetology/PlanetsCatalog
    public async Task<IActionResult> Index()
    {
        var count = await _context.PlanetsCatalog.CountAsync();
        ViewBag.RowsCount = count;

        ViewBag.PlanetNames = await _distinctSelectListService.GetDistinctSelectListAsync(c => c.PlLetter);
        ViewBag.TelescopNames = await _distinctSelectListService.GetDistinctSelectListAsync(c => c.DiscTelescope);
        ViewBag.DiscoveryMethod = await _distinctSelectListService.GetDistinctSelectListAsync(c => c.DiscoveryMethod);

        return View(await _context.PlanetsCatalog.Take(50).ToListAsync());
    }


    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        ViewBag.RowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "50";

        List<NASAExoplanetCatalog>? selectedList;

        try
        {
            selectedList = await _filterService.GetFilteredDataAsync(parameters);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching filtered data: {ex.Message}");
        }

        if (selectedList == null)
            return NotFound();

        var firstItem = selectedList.FirstOrDefault();

        ViewBag.RowsCount = firstItem?.PageCount ?? 1;
        ViewBag.AmountRowsResult = firstItem?.PageNumber ?? 0; // Using the PageNumber field of the database table to pass a value.
        ViewBag.Contorller = "PlanetsCatalog";

        try
        {
            var tableHtml = await this.RenderViewAsync("_PlanetsTableHeaders", selectedList, true);
            var paginationHtml = await this.RenderViewAsync("_PaginationLine", null, true);

            return Json(new { tableHtml, paginationHtml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"RenderViewAsync error: {ex.Message}");
        }
    }


    // GET: Planetology/PlanetsCatalog/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nASAExoplanetCatalog = await _context.PlanetsCatalog
            .FirstOrDefaultAsync(m => m.RowId == id);
        if (nASAExoplanetCatalog == null)
        {
            return NotFound();
        }

        return View(nASAExoplanetCatalog);
    }

    #region unnecessary methods
    // GET: Planetology/PlanetsCatalog/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Planetology/PlanetsCatalog/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RowId,PlName,Hostname,PlLetter,HdName,HipName,TicId,GaiaId,DefaultFlag,SySnum," +
        "SyPnum,SyMnum,CbFlag,DiscoveryMethod,DiscYear,DiscRefName,DiscPubDate,DiscLocale,DiscFacility,DiscTelescope,DiscInstrument," +
        "RvFlag,PulFlag,PtvFlag,TranFlag,AstFlag,ObmFlag,MicroFlag,EtvFlag,ImaFlag,DkinFlag,SolType,PlControvFlag,PlRefName,PlOrbPer," +
        "PlOrbPerErr1,PlOrbPerErr2,PlOrbPerLim,PlOrbSmax,PlOrbSmaxErr1,PlOrbSmaxErr2,PlOrbSmaxLim,PlRade,PlRadeErr1,PlRadeErr2," +
        "PlRadeLim,PlRadJ,PlRadJErr1,PlRadJErr2,PlRadJLim,PlMasse,PlMasseErr1,PlMasseErr2,PlMasseLim,PlMassJ,PlMassJErr1,PlMassJErr2," +
        "PlMassJLim,PlMsiniE,PlMsiniEErr1,PlMsiniEErr2,PlMsiniELim,PlMsiniJ,PlMsiniJErr1,PlMsiniJErr2,PlMsiniJLim,PlCMasse," +
        "PlCMasseErr1,PlCMasseErr2,PlCMasseLim,PlCMassJ,PlCMassJErr1,PlCMassJErr2,PlCMassJLim,PlBmasse,PlBmasseErr1,PlBmasseErr2," +
        "PlBmasseLim,PlBmassJ,PlBmassJErr1,PlBmassJErr2,PlBmassJLim,PlBmassProv,PlDens,PlDensErr1,PlDensErr2,PlDensLim,PlOrbEccen," +
        "PlOrbEccenErr1,PlOrbEccenErr2,PlOrbeccenLim,PlInsol,PlInsolErr1,PlInsolErr2,PlInsolLim,PlEqt,PlEqtErr1,PlEqtErr2,PlEqtLim," +
        "PlOrbincl,PlOrbinclErr1,PlOrbinclErr2,PlOrbinclLim,PlTranmid,PlTranmidErr1,PlTranmidErr2,PlTranmidLim,PlTsystemref,TtvFlag," +
        "PlImppar,PlImpparErr1,PlImpparErr2,PlImpparLim,PlTrandep,PlTrandepErr1,PlTrandepErr2,PlTrandepLim,PlTrandur,PlTrandurErr1," +
        "PlTrandurErr2,PlTrandurLim,PlRatdor,PlRatdorErr1,PlRatdorErr2,PlRatdorLim,PlRatror,PlRatrorErr1,PlRatrorErr2,PlRatrorLim," +
        "PlOccdep,PlOccdepErr1,PlOccdepErr2,PlOccdepLim,PlOrbtper,PlOrbtperErr1,PlOrbtperErr2,PlOrbtperLim,PlOrblper,PlOrblperErr1," +
        "PlOrblperErr2,PlOrblperLim,PlRvamp,PlRvampErr1,PlRvampErr2,PlRvampLim,PlProjobliq,PlProjobliqErr1,PlProjobliqErr2," +
        "PlProjobliqLim,PlTrueobliq,PlTrueobliqErr1,PlTrueobliqErr2,PlTrueobliqLim,StRefname,StSpectype,StTeff,StTeffErr1,StTeffErr2," +
        "StTeffLim,StRad,StRadErr1,StRadErr2,StRadLim,StMass,StMassErr1,StMassErr2,StMassLim,StMet,StMetErr1,StMetErr2,StMetLim," +
        "StMetratio,StLum,StLumErr1,StLumErr2,StLumLim,StLogg,StLoggErr1,StLoggErr2,StLoggLim,StAge,StAgeErr1,StAgeErr2,StAgeLim," +
        "StDens,StDensErr1,StDensErr2,StDensLim,StVsin,StVsinErr1,StVsinErr2,StVsinLim,StRotp,StRotpErr1,StRotpErr2,StRotpLim,StRadv," +
        "StRadvErr1,StRadvErr2,StRadvLim,SyRefName,Rastr,Ra,Decstr,Dec,Glat,Glon,Elat,Elon,SyPm,SyPmErr1,SyPmErr2,SyPmRa,SyPmRaErr1," +
        "SyPmRaErr2,SyPmDec,SyPmDecErr1,SyPmDecErr2,SyDist,SyDistErr1,SyDistErr2,SyPlx,SyPlxErr1,SyPlxErr2,SyBmag,SyBmagErr1," +
        "SyBmagErr2,SyVmag,SyVmagErr1,SyVmagErr2,SyJmag,SyJmagErr1,SyJmagErr2,SyHmag,SyHmagErr1,SyHmagErr2,SyKmag,SyKmagErr1," +
        "SyKmagErr2,SyUmag,SyUmagErr1,SyUmagErr2,SyGmag,SyGmagErr1,SyGmagErr2,SyRmag,SyRmagErr1,SyRmagErr2,SyImag,SyImagErr1," +
        "SyImagErr2,SyZmag,SyZmagErr1,SyZmagErr2,SyW1mag,SyW1magErr1,SyW1magErr2,SyW2mag,SyW2magErr1,SyW2magErr2,SyW3mag,SyW3magErr1," +
        "SyW3magErr2,SyW4mag,SyW4magErr1,SyW4magErr2,SyGaiaMag,SyGaiamagerr1,SyGaiamagerr2,SyIcmag,SyIcmagerr1,SyIcmagerr2,SyTmag," +
        "SyTmagerr1,SyTmagerr2,SyKepmag,SyKepmagerr1,SyKepmagerr2,Rowupdate,PlPubdate,Releasedate,PlNnotes,StNphot,StNrvc,StNspec," +
        "PlNespec,PlNtranspec,PlNdispec,PageNumber,PageCount")] NASAExoplanetCatalog nASAExoplanetCatalog)
    {
        if (ModelState.IsValid)
        {
            _context.Add(nASAExoplanetCatalog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(nASAExoplanetCatalog);
    }

    // GET: Planetology/PlanetsCatalog/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nASAExoplanetCatalog = await _context.PlanetsCatalog.FindAsync(id);
        if (nASAExoplanetCatalog == null)
        {
            return NotFound();
        }
        return View(nASAExoplanetCatalog);
    }

    // POST: Planetology/PlanetsCatalog/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("RowId,PlName,Hostname,PlLetter,HdName,HipName,TicId,GaiaId,DefaultFlag,SySnum," +
        "SyPnum,SyMnum,CbFlag,DiscoveryMethod,DiscYear,DiscRefName,DiscPubDate,DiscLocale,DiscFacility,DiscTelescope,DiscInstrument," +
        "RvFlag,PulFlag,PtvFlag,TranFlag,AstFlag,ObmFlag,MicroFlag,EtvFlag,ImaFlag,DkinFlag,SolType,PlControvFlag,PlRefName,PlOrbPer," +
        "PlOrbPerErr1,PlOrbPerErr2,PlOrbPerLim,PlOrbSmax,PlOrbSmaxErr1,PlOrbSmaxErr2,PlOrbSmaxLim,PlRade,PlRadeErr1,PlRadeErr2," +
        "PlRadeLim,PlRadJ,PlRadJErr1,PlRadJErr2,PlRadJLim,PlMasse,PlMasseErr1,PlMasseErr2,PlMasseLim,PlMassJ,PlMassJErr1,PlMassJErr2," +
        "PlMassJLim,PlMsiniE,PlMsiniEErr1,PlMsiniEErr2,PlMsiniELim,PlMsiniJ,PlMsiniJErr1,PlMsiniJErr2,PlMsiniJLim,PlCMasse," +
        "PlCMasseErr1,PlCMasseErr2,PlCMasseLim,PlCMassJ,PlCMassJErr1,PlCMassJErr2,PlCMassJLim,PlBmasse,PlBmasseErr1,PlBmasseErr2," +
        "PlBmasseLim,PlBmassJ,PlBmassJErr1,PlBmassJErr2,PlBmassJLim,PlBmassProv,PlDens,PlDensErr1,PlDensErr2,PlDensLim,PlOrbEccen," +
        "PlOrbEccenErr1,PlOrbEccenErr2,PlOrbeccenLim,PlInsol,PlInsolErr1,PlInsolErr2,PlInsolLim,PlEqt,PlEqtErr1,PlEqtErr2,PlEqtLim," +
        "PlOrbincl,PlOrbinclErr1,PlOrbinclErr2,PlOrbinclLim,PlTranmid,PlTranmidErr1,PlTranmidErr2,PlTranmidLim,PlTsystemref,TtvFlag," +
        "PlImppar,PlImpparErr1,PlImpparErr2,PlImpparLim,PlTrandep,PlTrandepErr1,PlTrandepErr2,PlTrandepLim,PlTrandur,PlTrandurErr1," +
        "PlTrandurErr2,PlTrandurLim,PlRatdor,PlRatdorErr1,PlRatdorErr2,PlRatdorLim,PlRatror,PlRatrorErr1,PlRatrorErr2,PlRatrorLim," +
        "PlOccdep,PlOccdepErr1,PlOccdepErr2,PlOccdepLim,PlOrbtper,PlOrbtperErr1,PlOrbtperErr2,PlOrbtperLim,PlOrblper,PlOrblperErr1," +
        "PlOrblperErr2,PlOrblperLim,PlRvamp,PlRvampErr1,PlRvampErr2,PlRvampLim,PlProjobliq,PlProjobliqErr1,PlProjobliqErr2," +
        "PlProjobliqLim,PlTrueobliq,PlTrueobliqErr1,PlTrueobliqErr2,PlTrueobliqLim,StRefname,StSpectype,StTeff,StTeffErr1,StTeffErr2," +
        "StTeffLim,StRad,StRadErr1,StRadErr2,StRadLim,StMass,StMassErr1,StMassErr2,StMassLim,StMet,StMetErr1,StMetErr2,StMetLim," +
        "StMetratio,StLum,StLumErr1,StLumErr2,StLumLim,StLogg,StLoggErr1,StLoggErr2,StLoggLim,StAge,StAgeErr1,StAgeErr2,StAgeLim," +
        "StDens,StDensErr1,StDensErr2,StDensLim,StVsin,StVsinErr1,StVsinErr2,StVsinLim,StRotp,StRotpErr1,StRotpErr2,StRotpLim,StRadv," +
        "StRadvErr1,StRadvErr2,StRadvLim,SyRefName,Rastr,Ra,Decstr,Dec,Glat,Glon,Elat,Elon,SyPm,SyPmErr1,SyPmErr2,SyPmRa,SyPmRaErr1," +
        "SyPmRaErr2,SyPmDec,SyPmDecErr1,SyPmDecErr2,SyDist,SyDistErr1,SyDistErr2,SyPlx,SyPlxErr1,SyPlxErr2,SyBmag,SyBmagErr1," +
        "SyBmagErr2,SyVmag,SyVmagErr1,SyVmagErr2,SyJmag,SyJmagErr1,SyJmagErr2,SyHmag,SyHmagErr1,SyHmagErr2,SyKmag,SyKmagErr1," +
        "SyKmagErr2,SyUmag,SyUmagErr1,SyUmagErr2,SyGmag,SyGmagErr1,SyGmagErr2,SyRmag,SyRmagErr1,SyRmagErr2,SyImag,SyImagErr1," +
        "SyImagErr2,SyZmag,SyZmagErr1,SyZmagErr2,SyW1mag,SyW1magErr1,SyW1magErr2,SyW2mag,SyW2magErr1,SyW2magErr2,SyW3mag,SyW3magErr1," +
        "SyW3magErr2,SyW4mag,SyW4magErr1,SyW4magErr2,SyGaiaMag,SyGaiamagerr1,SyGaiamagerr2,SyIcmag,SyIcmagerr1,SyIcmagerr2,SyTmag," +
        "SyTmagerr1,SyTmagerr2,SyKepmag,SyKepmagerr1,SyKepmagerr2,Rowupdate,PlPubdate,Releasedate,PlNnotes,StNphot,StNrvc,StNspec," +
        "PlNespec,PlNtranspec,PlNdispec,PageNumber,PageCount")] NASAExoplanetCatalog nASAExoplanetCatalog)
    {
        if (id != nASAExoplanetCatalog.RowId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(nASAExoplanetCatalog);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NASAExoplanetCatalogExists(nASAExoplanetCatalog.RowId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(nASAExoplanetCatalog);
    }

    // GET: Planetology/PlanetsCatalog/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nASAExoplanetCatalog = await _context.PlanetsCatalog
            .FirstOrDefaultAsync(m => m.RowId == id);
        if (nASAExoplanetCatalog == null)
        {
            return NotFound();
        }

        return View(nASAExoplanetCatalog);
    }

    // POST: Planetology/PlanetsCatalog/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var nASAExoplanetCatalog = await _context.PlanetsCatalog.FindAsync(id);
        if (nASAExoplanetCatalog != null)
        {
            _context.PlanetsCatalog.Remove(nASAExoplanetCatalog);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool NASAExoplanetCatalogExists(int id)
    {
        return _context.PlanetsCatalog.Any(e => e.RowId == id);
    }
    #endregion


    [HttpPost]
    public async Task<IActionResult> ImportData_OpenXml(string jobId)
    {
        var cts = _importCancellationService.GetOrCreateToken(jobId);

        try
        {
            await _excelImportService.ImportDataAsync(jobId, cts.Token);
            TempData["Message"] = "Import completed successfully!";
            return Ok();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499); // Non-standard "Client Closed Request"
        }
    }

    [HttpPost]
    public IActionResult CancelImport(string jobId)
    {
        _importCancellationService.Cancel(jobId);
        return Ok();
    }

}
