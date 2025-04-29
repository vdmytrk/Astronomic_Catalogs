using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers;

[Area("Catalogs")]
public class NGCICOpendatasoftsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NGCICOpendatasoftsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Catalogs/NGCICOpendatasofts
    public async Task<IActionResult> Index()
    {
        var countNGCTask = await _context.NGCIC_Catalog.CountAsync();
        var countNGCE_Task = await _context.NGCICOpendatasoft_E.CountAsync();

        var catalogItems = await _context.NGCIC_Catalog
            .OrderBy(x => x.NGC_IC)
            .ThenBy(x => x.Name)
            .Take(50)
            .ToListAsync();

        var constellationsTask = await _context.Constellations
            .Select(c => new
            {
                c.ShortName,
                c.LatineNameNominativeCase,
                c.EnglishName,
                c.UkraineName
            })
            .ToListAsync();


        ViewBag.RowsCount = countNGCTask + countNGCE_Task;
        ViewBag.Constellations = constellationsTask;

        return View(catalogItems);
    }


    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        //string rowOnPageCatalog = parameters["RowOnPageCatalog"].ToString() ?? "";
        //ViewBag.RowOnPageCatalog = rowOnPageCatalog == "100" ? 100 : 50;
        try
        {
            // Development block
            var parametersList = parameters.ToList();
            var firstParam = parametersList[1]; 
            var firstParamKey = firstParam.Key;   
            var firstParamValue = firstParam.Value; 

            List<NGCICOpendatasoft> selectedListNGCIC2000 = new();
            var countNGCTask = await _context.NGCIC_Catalog.CountAsync();
            var countNGCE_Task = await _context.NGCICOpendatasoft_E.CountAsync();

            selectedListNGCIC2000 = await _context.NGCIC_Catalog
                .OrderBy(x => x.NGC_IC)
                .ThenBy(x => x.Name)
                .Take(50)
                .ToListAsync();
            // The end of development block

            ViewBag.RowsCount = countNGCTask + countNGCE_Task;
            // _updateTable - це сервіс який потрібно передати як задежність!!!
            //selectedListNGCIC2000 = new(); // await _updateTable.GetDataWithFiltersNGC2000(parameters);
            //ViewBag.RowsCount = selectedListNGCIC2000.Count;
            //if (selectedListNGCIC2000 == null)
            //{
            //    return NotFound();
            //}
            return PartialView("_NGCICTableHeaders", selectedListNGCIC2000);
        }
        catch 
        {
            throw;
        }
    }

    #region Optional methods
    // GET: Catalogs/NGCICOpendatasofts/Details/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nGCICOpendatasoft = await _context.NGCIC_Catalog
            .FirstOrDefaultAsync(m => m.Id == id);
        if (nGCICOpendatasoft == null)
        {
            return NotFound();
        }

        return View(nGCICOpendatasoft);
    }

    // GET: Catalogs/NGCICOpendatasofts/Create
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Catalogs/NGCICOpendatasofts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,NGC_IC,Name,SubObject,Messier,Name_UK,Comment,OtherNames," +
        "NGC,IC,LimitAngDiameter,AngDiameter,ObjectTypeAbrev,ObjectType,ObjectTypeFull,SourceType,RA," +
        "RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,DEC,Declination,NS,DeclinationD," +
        "DeclinationM,DeclinationS,Constellation,MajorAxis,MinorAxis,PositionAngle,AppMag,AppMagFlag,BMag," +
        "VMag,JMag,HMag,KMag,SurfaceBrightness,HubbleOnlyGalaxies,CstarUMag,CstarBMag,CstarVMag,CstarNames," +
        "CommonNames,NedNotes,OpenngcNotes,Image,PageNumber,PageCount")] NGCICOpendatasoft nGCICOpendatasoft)
    {
        if (ModelState.IsValid)
        {
            _context.Add(nGCICOpendatasoft);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(nGCICOpendatasoft);
    }

    // GET: Catalogs/NGCICOpendatasofts/Edit/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nGCICOpendatasoft = await _context.NGCIC_Catalog.FindAsync(id);
        if (nGCICOpendatasoft == null)
        {
            return NotFound();
        }
        return View(nGCICOpendatasoft);
    }

    // POST: Catalogs/NGCICOpendatasofts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,NGC_IC,Name,SubObject,Messier,Name_UK,Comment,OtherNames," +
        "NGC,IC,LimitAngDiameter,AngDiameter,ObjectTypeAbrev,ObjectType,ObjectTypeFull,SourceType,RA," +
        "RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,DEC,Declination,NS,DeclinationD," +
        "DeclinationM,DeclinationS,Constellation,MajorAxis,MinorAxis,PositionAngle,AppMag,AppMagFlag,BMag," +
        "VMag,JMag,HMag,KMag,SurfaceBrightness,HubbleOnlyGalaxies,CstarUMag,CstarBMag,CstarVMag,CstarNames," +
        "CommonNames,NedNotes,OpenngcNotes,Image,PageNumber,PageCount")] NGCICOpendatasoft nGCICOpendatasoft)
    {
        if (id != nGCICOpendatasoft.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(nGCICOpendatasoft);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NGCICOpendatasoftExists(nGCICOpendatasoft.Id))
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
        return View(nGCICOpendatasoft);
    }

    // GET: Catalogs/NGCICOpendatasofts/Delete/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nGCICOpendatasoft = await _context.NGCIC_Catalog
            .FirstOrDefaultAsync(m => m.Id == id);
        if (nGCICOpendatasoft == null)
        {
            return NotFound();
        }

        return View(nGCICOpendatasoft);
    }

    // POST: Catalogs/NGCICOpendatasofts/Delete/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var nGCICOpendatasoft = await _context.NGCIC_Catalog.FindAsync(id);
        if (nGCICOpendatasoft != null)
        {
            _context.NGCIC_Catalog.Remove(nGCICOpendatasoft);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    #endregion

    private bool NGCICOpendatasoftExists(int id)
    {
        return _context.NGCIC_Catalog.Any(e => e.Id == id);
    }
}
