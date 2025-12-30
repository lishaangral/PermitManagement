using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Data;
using PemitManagement.Data.Enums;
using PemitManagement.Models;
using PemitManagement.ViewModels.Observations;

[Authorize]
public class ObservationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ObservationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Policy = "create_observations")]
    public async Task<IActionResult> Create()
    {
        var empNo = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(empNo))
            return Forbid();

        var employee = await _context.Employees
            .AsNoTracking()
            .Where(e => e.EmpNo == empNo)
            .Select(e => new EmployeeDetailsViewModel
            {
                EmployeeId = e.EmpNo,
                EmployeeName = string.IsNullOrWhiteSpace(e.Designation)
                    ? e.Name
                    : $"{e.Name} ({e.Designation})"
            })
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            TempData["ToastMessage"] = "Employee record not found.";
            return RedirectToAction("Index", "Home");
        }

        return View(employee);
    }

    [Authorize(Policy = "create_observations")]
    public IActionResult PermitDetails()
    {
        var model = HttpContext.Session.GetObject<PermitDetailsViewModel>("permit-details")
                    ?? new PermitDetailsViewModel();

        return View(model);
    }

    [Authorize(Policy = "create_observations")]
    [HttpPost]
    public async Task<IActionResult> FetchPermit(PermitDetailsViewModel model)
    {
        model.HasSearched = true;

        if (!ModelState.IsValid)
        {
            TempData["ToastMessage"] = "Permit number is required.";
            return View("PermitDetails", model);
        }

        var permit = await _context.Permits
            .Where(p => p.PermitNumber == model.PermitNumber)
            .Select(p => new
            {
                Permit = p,
                PermitType = p.PermitType.Name,
                LocationName = p.Location.Name,
                Refinery = p.Location.RefineryType,
                Status = p.Status
            })
            .FirstOrDefaultAsync();

        if (permit == null)
        {
            model.PermitFound = false;
            TempData["ToastMessage"] = "Permit not found.";
            return View("PermitDetails", model);
        }

        if (permit.Status == PermitStatus.Closed)
        {
            model.PermitFound = false;
            TempData["ToastMessage"] =
                "This permit is closed. Observations cannot be added to closed permits.";
            return View("PermitDetails", model);
        }

        // Fill model
        model.PermitFound = true;
        model.PermitId = permit.Permit.Id;
        model.PermitType = permit.PermitType;
        model.PermitTypeId = permit.Permit.PermitTypeId;
        model.AgencyName = permit.Permit.AgencyName;
        model.WorkOrderNumber = permit.Permit.WorkOrderNumber;
        model.ContractWorkerName = permit.Permit.ContractWorkerName;
        model.VisitedSite = permit.LocationName;
        model.Refinery = permit.Refinery;
        model.ExactLocation = permit.Permit.ExactLocation;
        model.ReceiverName = permit.Permit.ReceiverName;
        model.ReceiverEmployeeId = permit.Permit.ReceiverEmployeeId;

        // Issuers
        model.Issuers = await (
            from pi in _context.PermitIssuers
            join ir in _context.IssuerRoles on pi.IssuerRoleId equals ir.Id
            where pi.PermitId == permit.Permit.Id
            select new IssuerInfoViewModel
            {
                RoleName = ir.RoleName,
                EmployeeId = pi.EmployeeId,
                EmployeeName = pi.EmployeeName
            }
        ).ToListAsync();

        HttpContext.Session.SetObject("permit-details", model);
        return View("PermitDetails", model);
    }

    [Authorize(Policy = "create_observations")]
    public IActionResult ClearPermitDetails()
    {
        HttpContext.Session.Remove("permit-details");
        return RedirectToAction(nameof(PermitDetails));
    }

    [Authorize(Policy = "create_observations")]
    public async Task<IActionResult> Violations()
    {
        var permit = HttpContext.Session.GetObject<PermitDetailsViewModel>("permit-details");
        if (permit == null || !permit.PermitFound)
        {
            TempData["ToastMessage"] = "Please select a valid permit first.";
            return RedirectToAction(nameof(PermitDetails));
        }

        var permitEntity = await _context.Permits
            .AsNoTracking()
            .Where(p => p.PermitNumber == permit.PermitNumber)
            .Select(p => new { p.Id, PermitType = p.PermitType.Name, p.PermitTypeId, p.Status })
            .FirstAsync();

        if (permitEntity.Status == PermitStatus.Closed)
        {
            TempData["ToastMessage"] =
                "This permit has been closed. You cannot add observations.";
            HttpContext.Session.Remove("permit-details");
            HttpContext.Session.Remove("violations-page");
            return RedirectToAction(nameof(PermitDetails));
        }

        var violationsQuery =
            from ptv in _context.PermitTypeViolations
            join v in _context.Violations on ptv.ViolationId equals v.Id
            where ptv.PermitTypeId == permitEntity.PermitTypeId
                  && v.Active == true
            select v;

        var allViolations = await violationsQuery.ToListAsync();

        var vm = HttpContext.Session.GetObject<ViolationsPageViewModel>("violations-page");

        if (vm == null)
        {
            vm = new ViolationsPageViewModel
            {
                PermitId = permitEntity.Id,
                PermitTypeId = permitEntity.PermitTypeId,
                PermitTypeName = permitEntity.PermitType,
                SelectedCategories = new(),
                SelectedSeverities = new()
            };
        }

        vm.AllCategories = allViolations
            .Select(v => v.Category!)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        vm.AllSeverities = allViolations
            .Select(v => v.Severity!)
            .Distinct()
            .OrderBy(x => x)
            .ToList();


        vm.AvailableViolations = allViolations.Select(v => new ViolationListItemViewModel
        {
            ViolationId = v.Id,
            Name = v.Name,
            Category = v.Category ?? string.Empty,
            Severity = v.Severity ?? string.Empty,
            IsSelected = vm.SelectedViolations.Any(s => s.ViolationId == v.Id)
        }).ToList();

        HttpContext.Session.SetObject("violations-page", vm);
        return View(vm);
    }

    [Authorize(Policy = "create_observations")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitViolations(SubmitViolationsViewModel model)
    {
        var permitDetails = HttpContext.Session.GetObject<PermitDetailsViewModel>("permit-details");

        var permitStatus = await _context.Permits
            .Where(p => p.Id == model.PermitId)
            .Select(p => p.Status)
            .FirstOrDefaultAsync();

        if (permitStatus == PermitStatus.Closed)
        {
            TempData["ToastMessage"] =
                "This permit is already closed. Observation submission is not allowed.";
            return RedirectToAction(nameof(PermitDetails));
        }

        if (permitDetails == null)
        {
            TempData["ToastMessage"] = "Session expired. Please restart the observation.";
            return RedirectToAction(nameof(Create));
        }

        if (model.PermitId != permitDetails.PermitId)
        {
            TempData["ToastMessage"] = "Invalid permit context.";
            return RedirectToAction(nameof(PermitDetails));
        }

        if (model.Violations == null || model.Violations.Count == 0)
        {
            TempData["ToastMessage"] = "Please add details for at least one violation.";
            return RedirectToAction(nameof(Violations));
        }

        foreach (var v in model.Violations)
        {
            bool hasRemarks = !string.IsNullOrWhiteSpace(v.Remarks);
            bool hasAction = !string.IsNullOrWhiteSpace(v.ActionTaken);
            bool hasImages = v.Images != null && v.Images.Count > 0;

            if (!hasRemarks && !hasAction && !hasImages)
            {
                TempData["ToastMessage"] =
                    "Each selected violation must have remarks, action taken, or images.";
                return RedirectToAction(nameof(Violations));
            }

            if (v.Images != null)
            {
                if (v.Images.Count > 5)
                {
                    TempData["ToastMessage"] = "Maximum 5 images allowed per violation.";
                    return RedirectToAction(nameof(Violations));
                }

                foreach (var image in v.Images)
                {
                    if (image.Length > 5 * 1024 * 1024)
                    {
                        TempData["ToastMessage"] =
                            $"Image '{image.FileName}' exceeds 5 MB.";
                        return RedirectToAction(nameof(Violations));
                    }

                    var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
                    {
                        TempData["ToastMessage"] =
                            $"Invalid image format: {image.FileName}";
                        return RedirectToAction(nameof(Violations));
                    }
                }
            }
        }

        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var v in model.Violations)
            {
                var permitViolation = new PermitViolation
                {
                    PermitId = model.PermitId,
                    ViolationId = v.ViolationId,
                    Remarks = v.Remarks,
                    ActionTaken = v.ActionTaken
                };

                _context.PermitViolations.Add(permitViolation);
                await _context.SaveChangesAsync(); // get ID

                if (v.Images != null && v.Images.Any())
                {
                    var basePath = Path.Combine(
                        "wwwroot",
                        "uploads",
                        "permits",
                        model.PermitId.ToString(),
                        "violations",
                        v.ViolationId.ToString()
                    );

                    Directory.CreateDirectory(basePath);

                    foreach (var image in v.Images)
                    {
                        var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                        var fileName = $"violation_{v.ViolationId}_{Guid.NewGuid():N}{ext}";
                        var filePath = Path.Combine(basePath, fileName);

                        await using var stream = new FileStream(filePath, FileMode.Create);
                        await image.CopyToAsync(stream);

                        var relativePath = Path
                            .GetRelativePath("wwwroot", filePath)
                            .Replace("\\", "/");

                        _context.ViolationImages.Add(new ViolationImage
                        {
                            PermitViolationId = permitViolation.Id,
                            ImagePath = "/" + relativePath
                        });

                    }
                }
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            HttpContext.Session.Remove("permit-details");
            HttpContext.Session.Remove("violations-page");

            TempData["ToastMessage"] = "Observation submitted successfully.";

            return RedirectToAction("Index", "ReportAnalysis",
                new { permitId = model.PermitId });
        }
        catch (Exception)
        {
            await tx.RollbackAsync();
            TempData["ToastMessage"] = "Failed to submit observation.";
            return RedirectToAction(nameof(Violations));
        }
    }
}
