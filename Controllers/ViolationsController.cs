using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Data;
using PemitManagement.Models;
using PemitManagement.ViewModels.Violations;

[Authorize(Policy = "manage_violations")]
public class ViolationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ViolationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // INDEX (LIST + PAGINATION)
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        int pageSize = 10)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Violations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v =>
                v.Name.Contains(search) ||
                v.Category!.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var violations = await query
            .OrderBy(v => v.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new ViolationListItemViewModel
            {
                Id = (int)v.Id,
                Name = v.Name,
                Description = v.Description ?? "N/A",
                Category = v.Category ?? "N/A",
                Severity = v.Severity ?? "N/A",
                Active = v.Active == true,
                PermitTypes = _context.PermitTypeViolations
                    .Where(ptv => ptv.ViolationId == v.Id)
                    .Select(ptv => ptv.PermitType.Name)
                    .ToList()
            })
            .ToListAsync();

        // Needed by CREATE MODAL
        ViewBag.PermitTypes = await _context.PermitTypes
            .Where(p => p.Active == true)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var vm = new ManageViolationsViewModel
        {
            Violations = violations,
            Search = search,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(vm);
    }

    // CREATE (MODAL POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViolationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ToastMessage"] = "Invalid violation details.";
            return RedirectToAction(nameof(Index));
        }

        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var violation = new Violation
            {
                Name = model.Name.Trim(),
                Description = model.Description,
                Category = model.Category,
                Severity = model.Severity,
                Active = model.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.Violations.Add(violation);
            await _context.SaveChangesAsync();

            // Map permit types
            if (model.PermitTypeIds != null && model.PermitTypeIds.Any())
            {
                foreach (var ptId in model.PermitTypeIds)
                {
                    _context.PermitTypeViolations.Add(new PermitTypeViolation
                    {
                        PermitTypeId = (uint)ptId,
                        ViolationId = violation.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["ToastMessage"] = "Violation created successfully.";
        }
        catch
        {
            await tx.RollbackAsync();
            TempData["ToastMessage"] = "Failed to create violation.";
        }

        return RedirectToAction(nameof(Index));
    }

    // DELETE (SAFE STRATEGY)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var violation = await _context.Violations
            .Include(v => v.PermitViolations)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (violation == null)
            return NotFound();

        // SAFETY RULE:
        // If violation already used → soft-disable
        if (violation.PermitViolations.Any())
        {
            violation.Active = false;
            violation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["ToastMessage"] =
                "Violation is already used. It has been marked inactive instead of deleted.";

            return RedirectToAction(nameof(Index));
        }

        // Hard delete if unused
        _context.PermitTypeViolations.RemoveRange(
            _context.PermitTypeViolations.Where(x => x.ViolationId == id));

        _context.Violations.Remove(violation);
        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = "Violation deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
