using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Data;
using PemitManagement.Data.Enums;
using PemitManagement.Models;
using PemitManagement.ViewModels.Observations;
using PemitManagement.ViewModels.Permits;
using System.Text;

[Authorize]
public class PermitsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PermitsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(PermitFilterViewModel filters)
    {
        if (filters.Page <= 0) filters.Page = 1;
        if (filters.PageSize <= 0) filters.PageSize = 10;

        await ResolveDatePreset(filters);

        var query = _context.Permits
            .Include(p => p.PermitType)
            .Include(p => p.Location)
            .AsQueryable();

        query = ApplyFilters(query, filters);

        var totalCount = await query.CountAsync();

        var violationCounts = await _context.PermitViolations
            .GroupBy(v => v.PermitId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        var permits = await query
            .OrderByDescending(p => p.WorkDate)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(p => new PermitListItemViewModel
            {
                Id = (int)p.Id,
                PermitNumber = p.PermitNumber,
                Employee = $"{p.EmployeeName} ({p.EmployeeId})",
                Type = p.PermitType.Name,
                Location = p.Location.Name,
                Agency = p.AgencyName ?? "",
                WorkDate = p.WorkDate.HasValue
                    ? p.WorkDate.Value.ToDateTime(new TimeOnly(0, 0))
                    : null,
                Status = p.Status,
                ViolationCount = violationCounts.ContainsKey(p.Id)
                    ? violationCounts[p.Id]
                    : 0
            })
            .ToListAsync();

        var vm = new PermitIndexViewModel
        {
            Permits = permits,
            Filters = filters,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize),

            Employees = await _context.Permits
                .Select(p => new { p.EmployeeId, p.EmployeeName })
                .Distinct()
                .OrderBy(x => x.EmployeeName)
                .Select(x => new SelectListItem
                {
                    Value = x.EmployeeId,
                    Text = $"{x.EmployeeName} ({x.EmployeeId})"
                }).ToListAsync(),

            PermitTypes = await _context.PermitTypes
                .Where(x => x.Active == true)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync(),

            Locations = await _context.Locations
                .Where(x => x.Active == true)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync()
        };

        return View(vm);
    }

    private IQueryable<Permit> ApplyFilters(
        IQueryable<Permit> query,
        PermitFilterViewModel filters)
    {
        if (filters.Status.HasValue)
        {
            query = query.Where(p => p.Status == filters.Status.Value);
        }
        if (!string.IsNullOrWhiteSpace(filters.PermitNumber))
            query = query.Where(p => p.PermitNumber.Contains(filters.PermitNumber));

        if (!string.IsNullOrWhiteSpace(filters.EmployeeId))
            query = query.Where(p => p.EmployeeId == filters.EmployeeId);

        if (filters.PermitTypeId.HasValue)
            query = query.Where(p => p.PermitTypeId == filters.PermitTypeId);

        if (filters.LocationId.HasValue)
            query = query.Where(p => p.LocationId == filters.LocationId);

        if (filters.FromDate.HasValue)
            query = query.Where(p =>
                p.WorkDate.HasValue &&
                p.WorkDate.Value >= DateOnly.FromDateTime(filters.FromDate.Value));

        if (filters.ToDate.HasValue)
            query = query.Where(p =>
                p.WorkDate.HasValue &&
                p.WorkDate.Value <= DateOnly.FromDateTime(filters.ToDate.Value));

        if (filters.ViolationFilter == "With")
        {
            query = query.Where(p =>
                _context.PermitViolations
                    .Any(v => v.PermitId == p.Id));
        }
        else if (filters.ViolationFilter == "Without")
        {
            query = query.Where(p =>
                !_context.PermitViolations
                    .Any(v => v.PermitId == p.Id));
        }

        return query;
    }

    private async Task ResolveDatePreset(PermitFilterViewModel filters)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var minDate = await _context.Permits
            .Where(p => p.WorkDate.HasValue)
            .MinAsync(p => p.WorkDate!.Value);

        var minDateTime = minDate.ToDateTime(new TimeOnly(0, 0));
        var todayDateTime = today.ToDateTime(new TimeOnly(23, 59));

        if (string.IsNullOrEmpty(filters.DatePreset) || filters.DatePreset == "AllTime")
        {
            filters.FromDate = minDateTime;
            filters.ToDate = todayDateTime;
        }
        else if (filters.DatePreset == "Last7Days")
        {
            filters.FromDate = today.AddDays(-7).ToDateTime(new TimeOnly(0, 0));
            filters.ToDate = todayDateTime;
        }
        else if (filters.DatePreset == "Last30Days")
        {
            filters.FromDate = today.AddDays(-30).ToDateTime(new TimeOnly(0, 0));
            filters.ToDate = todayDateTime;
        }
        else if (filters.DatePreset == "ThisMonth")
        {
            filters.FromDate = new DateOnly(today.Year, today.Month, 1)
                .ToDateTime(new TimeOnly(0, 0));
            filters.ToDate = todayDateTime;
        }

        if (filters.FromDate < minDateTime)
            filters.FromDate = minDateTime;

        if (filters.ToDate > DateTime.Today)
            filters.ToDate = DateTime.Today;
    }

    public async Task<IActionResult> ExportCsv(PermitFilterViewModel filters)
    {
        await ResolveDatePreset(filters);

        var query = ApplyFilters(
            _context.Permits
                .Include(p => p.PermitType)
                .Include(p => p.Location),
            filters);

        var rows = await query.Select(p => new PermitExportRow
        {
            PermitNumber = p.PermitNumber,
            Employee = $"{p.EmployeeName} ({p.EmployeeId})",
            PermitType = p.PermitType.Name,
            Location = p.Location.Name,
            Agency = p.AgencyName ?? "",
            WorkDate = p.WorkDate.HasValue
                ? p.WorkDate.Value.ToString("dd-MM-yyyy")
                : "",
            Status = p.Status,
            ViolationCount = _context.PermitViolations.Count(v => v.PermitId == p.Id)
        }).ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("PermitNumber,Employee,PermitType,Location,Agency,Date,Status,Violations");

        foreach (var r in rows)
        {
            sb.AppendLine(
                $"{r.PermitNumber}," +
                $"\"{r.Employee}\"," +
                $"{r.PermitType}," +
                $"{r.Location}," +
                $"{r.Agency}," +
                $"\t{r.WorkDate}," +
                $"{r.Status}," +
                $"{r.ViolationCount}"
            );
        }

        return File(
            Encoding.UTF8.GetBytes(sb.ToString()),
            "text/csv",
            $"Permits_{DateTime.Now:yyyyMMddHHmm}.csv"
        );
    }

    public async Task<IActionResult> ExportExcel(PermitFilterViewModel filters)
    {
        await ResolveDatePreset(filters);

        var query = ApplyFilters(
            _context.Permits
                .Include(p => p.PermitType)
                .Include(p => p.Location),
            filters);

        var data = await query.Select(p => new PermitExportRow
        {
            PermitNumber = p.PermitNumber,
            Employee = $"{p.EmployeeName} ({p.EmployeeId})",
            PermitType = p.PermitType.Name,
            Location = p.Location.Name,
            Agency = p.AgencyName ?? "",
            WorkDate = p.WorkDate.HasValue
                ? p.WorkDate.Value.ToString("dd-MM-yyyy")
                : "",
            Status = p.Status,
            ViolationCount = _context.PermitViolations.Count(v => v.PermitId == p.Id)
        }).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Permits");

        ws.Cell(1, 1).InsertTable(data);
        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Permits_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        );
    }

    [Authorize(Policy = "close_permits")]
    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var permit = await _context.Permits
            .Include(p => p.PermitViolations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (permit == null)
            return NotFound();

        bool isClosing = permit.Status == PermitStatus.Open;

        // Toggle permit
        permit.Status = isClosing
            ? PermitStatus.Closed
            : PermitStatus.Open;

        permit.ClosedAt = isClosing ? DateTime.UtcNow : null;
        permit.ClosedBy = isClosing ? User.Identity!.Name : null;

        // Toggle related observations SAFELY
        foreach (var v in permit.PermitViolations)
        {
            v.Status = isClosing
                ? ObservationStatus.Closed
                : ObservationStatus.Open;

            v.ClosedAt = isClosing ? permit.ClosedAt : null;
            v.ClosedBy = isClosing ? permit.ClosedBy : null;
        }

        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = isClosing
            ? $"Permit {permit.PermitNumber} closed."
            : $"Permit {permit.PermitNumber} reopened.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "create_permits")]
    public IActionResult Create()
    {
        return View();
    }
    public async Task<IActionResult> Permit(int id)
    {
        var permit = await _context.Permits
            .Include(p => p.PermitType)
            .Include(p => p.Location)
            .Include(p => p.PermitViolations)
                .ThenInclude(pv => pv.Violation)
            .Include(p => p.PermitViolations)
                .ThenInclude(pv => pv.ViolationImages)
            .FirstOrDefaultAsync(p => p.Id == id);


        if (permit == null)
            return NotFound();

        var issuers = await (
            from pi in _context.PermitIssuers
            join ir in _context.IssuerRoles on pi.IssuerRoleId equals ir.Id
            where pi.PermitId == id
            select new IssuerInfoViewModel
            {
                RoleName = ir.RoleName,
                EmployeeId = pi.EmployeeId,
                EmployeeName = pi.EmployeeName
            }).ToListAsync();

        var isPermitClosed = permit.Status == PermitStatus.Closed;

        var violations = permit.PermitViolations.Select(v => new PermitViolationViewModel
        {
            PermitViolationId = (int)v.Id,
            ViolationName = v.Violation.Name,
            Category = v.Violation.Category ?? "N/A",
            Severity = v.Violation.Severity ?? "N/A",
            Remarks = v.Remarks ?? "N/A",
            ActionTaken = v.ActionTaken ?? "N/A",
            CreatedAt = v.CreatedAt,
            ClosedAt = v.ClosedAt,
            Status = v.Status,
            IsPermitClosed = isPermitClosed,

            Images = v.ViolationImages
                .Select(i => i.ImagePath)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .ToList()
                }).ToList();

        var vm = new DetailsViewModel
        {
            PermitId = (int)permit.Id,
            PermitNumber = permit.PermitNumber,
            PermitType = permit.PermitType.Name,
            Location = permit.Location.Name,
            Refinery = (permit.Location.RefineryType ?? "N/A").ToString(),
            WorkDate = permit.WorkDate?.ToDateTime(TimeOnly.MinValue),
            ExactLocation = permit.ExactLocation ?? "N/A",
            AgencyName = permit.AgencyName ?? "N/A",
            WorkOrderNumber = permit.WorkOrderNumber ?? "N/A",
            ContractWorkerName = permit.ContractWorkerName ?? "N/A",
            EmployeeName = permit.EmployeeName,
            EmployeeId = permit.EmployeeId,
            ReceiverName = permit.ReceiverName ?? "N/A",
            ReceiverEmployeeId = permit.ReceiverEmployeeId ?? "N/A",
            Status = permit.Status,
            LatestObservationAt = violations.Any()
                ? violations.Max(v => v.CreatedAt)
                : null,
            Issuers = issuers,
            OpenViolations = violations.Where(v => v.Status == ObservationStatus.Open).ToList(),
            ClosedViolations = violations.Where(v => v.Status == ObservationStatus.Closed).ToList(),

        };

        return View(vm);
    }

    [Authorize(Policy = "close_permits")]
    [HttpPost]
    public async Task<IActionResult> ToggleObservation(int id)
    {
        var v = await _context.PermitViolations
            .Include(x => x.Permit)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (v == null) return NotFound();

        if (v.Permit.Status == PermitStatus.Closed)
        {
            TempData["ToastMessage"] =
                "Observations cannot be modified when the permit is closed.";
            return RedirectToAction(nameof(Permit), new { id = v.PermitId });
        }

        bool closing = v.Status == ObservationStatus.Open;

        v.Status = closing ? ObservationStatus.Closed : ObservationStatus.Open;
        v.ClosedAt = closing ? DateTime.UtcNow : null;
        v.ClosedBy = closing ? User.Identity!.Name : null;

        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = closing
            ? "Observation closed."
            : "Observation reopened.";

        return RedirectToAction(nameof(Permit), new { id = v.PermitId });
    }

}
