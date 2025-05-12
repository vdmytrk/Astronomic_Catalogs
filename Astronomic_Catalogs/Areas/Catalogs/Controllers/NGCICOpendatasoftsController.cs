using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Astronomic_Catalogs.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers;

[Area("Catalogs")]
public class NGCICOpendatasoftsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly INGCICFilterService _filterService;
    private readonly IMapper _mapper;

    public NGCICOpendatasoftsController(ApplicationDbContext context, INGCICFilterService filterService, IMapper mapper)
    {
        _context = context;
        _filterService = filterService;
        _mapper = mapper;
    }

    // GET: Catalogs/NGCICOpendatasofts
    public async Task<IActionResult> Index()
    {
        var countNGCTask = await _context.NGCIC_Catalog.CountAsync();
        var countNGCE_Task = await _context.NGCICOpendatasoft_E.CountAsync();

        var catalogItems = await _context.NGCIC_Catalog
            .OrderByDescending(x => x.NGC_IC)
            .ThenBy(x => x.Name)
            .Take(50)
            .ToListAsync();

        var catalogViewModels = _mapper.Map<List<NGCICViewModel>>(catalogItems);

        var constellations = await _context.Constellations
            .Select(c => new
            {
                c.ShortName,
                c.LatineNameNominativeCase,
                c.EnglishName,
                c.UkraineName
            })
            .ToListAsync();


        ViewBag.RowsCount = countNGCTask + countNGCE_Task;
        ViewBag.Constellations = constellations;

        return View(catalogViewModels);
    }


    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        ViewBag.RowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "50";

        List<NGCICOpendatasoft>? selectedList;

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

        var viewModelList = _mapper.Map<List<NGCICViewModel>>(selectedList);
        var firstItem = viewModelList.FirstOrDefault();

        ViewBag.RowsCount = firstItem?.PageCount ?? 1;
        ViewBag.AmountRowsResult = firstItem?.PageNumber ?? 0; // Using the PageNumber field of the database table to pass a value.
        ViewBag.Contorller = "NGCICOpendatasofts";

        try
        {
            var tableHtml = await this.RenderViewAsync("_NGCICTableHeaders", viewModelList, true);
            var paginationHtml = await this.RenderViewAsync("_PaginationLine", null, true);

            return Json(new { tableHtml, paginationHtml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"RenderViewAsync error: {ex.Message}");
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

        var entity = await _context.NGCIC_Catalog.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null) return NotFound();

        var viewModel = _mapper.Map<NGCICViewModel>(entity);
        return View(viewModel);
    }

    // GET: Catalogs/NGCICOpendatasofts/Create
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public IActionResult Create()
    {
        var model = new NGCICViewModel();
        return View(model);
    }

    // POST: Catalogs/NGCICOpendatasofts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NGCICViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var entity = _mapper.Map<NGCICOpendatasoft>(viewModel);
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
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

        var entity = await _context.NGCIC_Catalog.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null) return NotFound();

        var viewModel = _mapper.Map<NGCICViewModel>(entity);
        return View(viewModel);
    }

    // POST: Catalogs/NGCICOpendatasofts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NGCICViewModel viewModel)
    {
        if (id != viewModel.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var entity = _mapper.Map<NGCICOpendatasoft>(viewModel);
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NGCICOpendatasoftExists(viewModel.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
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

        var entity = await _context.NGCIC_Catalog.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null) return NotFound();

        var viewModel = _mapper.Map<NGCICViewModel>(entity);
        return View(viewModel);
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
