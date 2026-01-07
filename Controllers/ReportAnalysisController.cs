using System.Text;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Data;
using PemitManagement.Data.Enums;
using PemitManagement.ViewModels.ReportAnalysis;
using System.Globalization;

[Authorize]
public class ReportAnalysisController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportAnalysisController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(ReportAnalysisFilterViewModel filters)
    {
        // defaults
        filters.Page = filters.Page <= 0 ? 1 : filters.Page;
        filters.PageSize = filters.PageSize <= 0 ? 10 : filters.PageSize;

        // default date range: last 30 days
        if (!filters.FromDate.HasValue || !filters.ToDate.HasValue)
        {
            filters.ToDate = DateTime.Today;
            filters.FromDate = DateTime.Today.AddDays(-30);
        }

        var baseQuery =
            from pv in _context.PermitViolations
            join p in _context.Permits on pv.PermitId equals p.Id
            join v in _context.Violations on pv.ViolationId equals v.Id
            join l in _context.Locations on p.LocationId equals l.Id
            where pv.CreatedAt >= filters.FromDate
               && pv.CreatedAt <= filters.ToDate
            select new
            {
                pv,
                p,
                v,
                l
            };

        // filters
        if (!string.IsNullOrWhiteSpace(filters.PermitNumber))
            baseQuery = baseQuery.Where(x => x.p.PermitNumber.Contains(filters.PermitNumber));

        if (!string.IsNullOrWhiteSpace(filters.WorkOrderNumber))
            baseQuery = baseQuery.Where(x => x.p.WorkOrderNumber!.Contains(filters.WorkOrderNumber));

        if (filters.PermitTypeId.HasValue)
            baseQuery = baseQuery.Where(x => x.p.PermitTypeId == filters.PermitTypeId);

        if (filters.LocationId.HasValue)
            baseQuery = baseQuery.Where(x => x.p.LocationId == filters.LocationId);

        if (!string.IsNullOrWhiteSpace(filters.RefineryType))
            baseQuery = baseQuery.Where(x => x.l.RefineryType == filters.RefineryType);

        if (filters.ViolationId.HasValue)
            baseQuery = baseQuery.Where(x => x.v.Id == filters.ViolationId);

        var data = await baseQuery.ToListAsync();

        var totalRecords = await baseQuery.CountAsync();

        var rows = await baseQuery
            .OrderByDescending(x => x.pv.CreatedAt)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(x => new ReportViolationRowVM
            {
                PermitId = (int)x.p.Id,
                PermitNumber = x.p.PermitNumber,
                Employee = $"{x.p.EmployeeName} ({x.p.EmployeeId})",
                Agency = x.p.AgencyName ?? "N/A",
                WorkOrder = x.p.WorkOrderNumber ?? "N/A",
                Violation = x.v.Name,
                Severity = x.v.Severity!,
                ObservationStatus = x.pv.Status,
                CreatedAt = x.pv.CreatedAt
            })
            .ToListAsync();

        var vm = new ReportAnalysisViewModel
        {
            Filters = filters,
            Rows = rows,
            TotalRecords = totalRecords,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize)
        };

        // SUMMARY (BY REPORT TYPE)
        switch (filters.ReportType)
        {
            case ReportType.ByEmployee:
                vm.EmployeeSummary = data
                    .GroupBy(x => new { x.p.EmployeeId, x.p.EmployeeName })
                    .Select(g => new EmployeeReportSummaryVM
                    {
                        EmployeeName = g.Key.EmployeeName,
                        TotalPermits = g.Select(x => x.p.Id).Distinct().Count(),
                        PermitsWithViolations = g.Select(x => x.p.Id).Distinct().Count(),
                        TotalViolations = g.Count(),
                        ActionTaken = g.Count(x => !string.IsNullOrWhiteSpace(x.pv.ActionTaken)),
                        FirstPermit = g.Min(x => x.pv.CreatedAt),
                        LastPermit = g.Max(x => x.pv.CreatedAt),
                        EntryDays = g.Select(x => x.pv.CreatedAt.Date).Distinct().Count()
                    })
                    .ToList();
                break;

            case ReportType.ByAgency:
                vm.AgencySummary = data
                    .GroupBy(x => x.p.AgencyName ?? "N/A")
                    .Select(g => new AgencyReportSummaryVM
                    {
                        AgencyName = g.Key,
                        TotalPermits = g.Select(x => x.p.Id).Distinct().Count(),
                        PermitsWithViolations = g.Select(x => x.p.Id).Distinct().Count(),
                        TotalViolations = g.Count(),
                        WorkOrders = g.Select(x => x.p.WorkOrderNumber).Distinct().Count()
                    })
                    .ToList();
                break;

            case ReportType.ByRefineryType:
                vm.RefinerySummary = data
                    .GroupBy(x => x.l.RefineryType)
                    .Select(g => new RefineryReportSummaryVM
                    {
                        RefineryType = g.Key!,
                        TotalPermits = g.Select(x => x.p.Id).Distinct().Count(),
                        PermitsWithViolations = g.Select(x => x.p.Id).Distinct().Count(),
                        TotalViolations = g.Count()
                    })
                    .ToList();                
                break;

            case ReportType.ByViolation:
                vm.ViolationSummary = data
                    .GroupBy(x => new { x.v.Name, x.v.Severity })
                    .Select(g => new ViolationReportSummaryVM
                    {
                        ViolationName = g.Key.Name,
                        Severity = g.Key.Severity ?? "",
                        TotalPermits = g.Select(x => x.p.Id).Distinct().Count(),
                        PermitsWithViolations = g.Select(x => x.p.Id).Distinct().Count(),
                        TotalViolations = g.Count()
                    })
                    .ToList();
                break;

            default: // Daily / Weekly / Monthly
                vm.TimeSummary = data
                    .GroupBy(x =>
                        filters.ReportType == ReportType.Daily
                            ? x.pv.CreatedAt.ToString("dd MMM yyyy")
                            : filters.ReportType == ReportType.Weekly
                                ? $"Week {x.pv.CreatedAt:yyyy}-W{CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                                        x.pv.CreatedAt,
                                        CalendarWeekRule.FirstFourDayWeek,
                                        DayOfWeek.Monday)}"
                                : x.pv.CreatedAt.ToString("MMM yyyy"))
                    .Select(g => new TimeReportSummaryVM
                    {
                        PeriodLabel = g.Key,
                        TotalPermits = g.Select(x => x.p.Id).Distinct().Count(),
                        PermitsWithViolations = g.Select(x => x.p.Id).Distinct().Count(),
                        TotalViolations = g.Count()
                    })
                    .ToList();
                break;
        }

        // CHART DATA (BY REPORT TYPE)

        switch (filters.ReportType)
        {
            case ReportType.ByEmployee:
                vm.ChartLabels = await baseQuery
                    .Select(x => x.p.EmployeeName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                vm.ChartPermits = await baseQuery
                    .GroupBy(x => x.p.EmployeeName)
                    .Select(g => g.Select(x => x.p.Id).Distinct().Count())
                    .ToListAsync();

                vm.ChartViolations = await baseQuery
                    .GroupBy(x => x.p.EmployeeName)
                    .Select(g => g.Count())
                    .ToListAsync();
                break;

            case ReportType.ByAgency:
                vm.ChartLabels = await baseQuery
                    .Select(x => x.p.AgencyName ?? "Unknown")
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                vm.ChartPermits = await baseQuery
                    .GroupBy(x => x.p.AgencyName)
                    .Select(g => g.Select(x => x.p.Id).Distinct().Count())
                    .ToListAsync();

                vm.ChartViolations = await baseQuery
                    .GroupBy(x => x.p.AgencyName)
                    .Select(g => g.Count())
                    .ToListAsync();
                break;

            case ReportType.ByRefineryType:
                vm.ChartLabels = await baseQuery
                    .Select(x => x.l.RefineryType ?? "N/A")
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                vm.ChartPermits = await baseQuery
                    .GroupBy(x => x.l.RefineryType)
                    .Select(g => g.Select(x => x.p.Id).Distinct().Count())
                    .ToListAsync();

                vm.ChartViolations = await baseQuery
                    .GroupBy(x => x.l.RefineryType)
                    .Select(g => g.Count())
                    .ToListAsync();
                break;

            default: // Time-based
                vm.ChartLabels = await baseQuery
                    .GroupBy(x => x.pv.CreatedAt.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => g.Key.ToString("dd MMM"))
                    .ToListAsync();

                vm.ChartPermits = await baseQuery
                    .GroupBy(x => x.pv.CreatedAt.Date)
                    .Select(g => g.Select(x => x.p.Id).Distinct().Count())
                    .ToListAsync();

                vm.ChartViolations = await baseQuery
                    .GroupBy(x => x.pv.CreatedAt.Date)
                    .Select(g => g.Count())
                    .ToListAsync();
                break;
        }

        vm.TotalRecords = data.Count;

        vm.Rows = data
            .OrderByDescending(x => x.pv.CreatedAt)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(x => new ReportViolationRowVM
            {
                PermitId = (int)x.p.Id,
                PermitNumber = x.p.PermitNumber,
                Employee = $"{x.p.EmployeeName} ({x.p.EmployeeId})",
                Agency = x.p.AgencyName ?? "",
                WorkOrder = x.p.WorkOrderNumber ?? "",
                Violation = x.v.Name,
                Category = x.v.Category ?? "",
                Severity = x.v.Severity ?? "",
                ObservationStatus = x.pv.Status,
                PermitStatus = x.p.Status,
                CreatedAt = x.pv.CreatedAt
            })
            .ToList();

        vm.TotalViolations = await baseQuery.CountAsync();

        vm.PermitsWithViolations = await baseQuery
            .Select(x => x.p.Id)
            .Distinct()
            .CountAsync();

        vm.TotalPermits = await _context.Permits
            .Where(p =>
                p.WorkDate >= DateOnly.FromDateTime(filters.FromDate.Value) &&
                p.WorkDate <= DateOnly.FromDateTime(filters.ToDate.Value))
            .CountAsync();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv(ReportAnalysisFilterViewModel filters)
    {
        var data = await GetReportData(filters);

        var sb = new StringBuilder();

        sb.AppendLine("Permit Number,Employee,Agency,Work Order,Violation,Category,Severity,Status,Created At");

        foreach (var r in data)
        {
            sb.AppendLine(
                $"{r.PermitNumber}," +
                $"{Escape(r.Employee)}," +
                $"{Escape(r.Agency)}," +
                $"{Escape(r.WorkOrder)}," +
                $"{Escape(r.Violation)}," +
                $"{Escape(r.Category)}," +
                $"{r.Severity}," +
                $"{r.ObservationStatus}," +
                $"\t{r.CreatedAt:yyyy-MM-dd HH:mm}"
            );
        }

        return File(
            Encoding.UTF8.GetBytes(sb.ToString()),
            "text/csv",
            $"ReportAnalysis_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        );
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(ReportAnalysisFilterViewModel filters)
    {
        var data = await GetReportData(filters);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Report Analysis");

        // Header
        ws.Cell(1, 1).Value = "Permit Number";
        ws.Cell(1, 2).Value = "Employee";
        ws.Cell(1, 3).Value = "Agency";
        ws.Cell(1, 4).Value = "Work Order";
        ws.Cell(1, 5).Value = "Violation";
        ws.Cell(1, 6).Value = "Category";
        ws.Cell(1, 7).Value = "Severity";
        ws.Cell(1, 8).Value = "Status";
        ws.Cell(1, 9).Value = "Created At";

        ws.Range(1, 1, 1, 9).Style.Font.Bold = true;

        int row = 2;
        foreach (var r in data)
        {
            ws.Cell(row, 1).Value = r.PermitNumber;
            ws.Cell(row, 2).Value = r.Employee;
            ws.Cell(row, 3).Value = r.Agency;
            ws.Cell(row, 4).Value = r.WorkOrder;
            ws.Cell(row, 5).Value = r.Violation;
            ws.Cell(row, 6).Value = r.Category;
            ws.Cell(row, 7).Value = r.Severity;
            ws.Cell(row, 8).Value = r.ObservationStatus.ToString();
            ws.Cell(row, 9).Value = r.CreatedAt.ToString("dd MMM yyyy HH:mm");

            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"ReportAnalysis_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        );
    }

    private async Task<List<ReportViolationRowVM>> GetReportData(
    ReportAnalysisFilterViewModel filters)
    {
        if (!filters.FromDate.HasValue || !filters.ToDate.HasValue)
        {
            filters.ToDate = DateTime.Today;
            filters.FromDate = DateTime.Today.AddDays(-30);
        }

        var query =
            from pv in _context.PermitViolations
            join p in _context.Permits on pv.PermitId equals p.Id
            join v in _context.Violations on pv.ViolationId equals v.Id
            join l in _context.Locations on p.LocationId equals l.Id
            where pv.CreatedAt >= filters.FromDate
               && pv.CreatedAt <= filters.ToDate
            select new
            {
                pv,
                p,
                v,
                l
            };

        if (!string.IsNullOrWhiteSpace(filters.PermitNumber))
            query = query.Where(x => x.p.PermitNumber.Contains(filters.PermitNumber));

        if (!string.IsNullOrWhiteSpace(filters.WorkOrderNumber))
            query = query.Where(x => x.p.WorkOrderNumber!.Contains(filters.WorkOrderNumber));

        if (filters.ViolationId.HasValue)
            query = query.Where(x => x.v.Id == filters.ViolationId);

        var data = await query
            .OrderByDescending(x => x.pv.CreatedAt)
            .Select(x => new ReportViolationRowVM
            {
                PermitId = (int)x.p.Id,
                PermitNumber = x.p.PermitNumber,
                Employee = $"{x.p.EmployeeName} ({x.p.EmployeeId})",
                Agency = x.p.AgencyName ?? "",
                WorkOrder = x.p.WorkOrderNumber ?? "",
                Violation = x.v.Name,
                Category = x.v.Category ?? "",
                Severity = x.v.Severity ?? "",
                ObservationStatus = x.pv.Status,
                CreatedAt = x.pv.CreatedAt
            })
            .ToListAsync();

        return data;
    }

}
