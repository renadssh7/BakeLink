using Bake_Link.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Admin;

[Area("Admin")]
[Route("admin/reports")]
public class ReportController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public ReportController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("performance")]
    public async Task<IActionResult> Performance([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var from = dateFrom ?? new DateTime(DateTime.UtcNow.Year, 1, 1);
        var to = dateTo ?? new DateTime(DateTime.UtcNow.Year, 12, 31);

        ViewData["Stats"] = new
        {
            total_orders = await _dbContext.Orders.CountAsync(x => x.OrderDate >= from && x.OrderDate <= to),
            total_revenue = await _dbContext.Payments
                .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            active_suppliers = await _dbContext.Suppliers.CountAsync(x => x.ApprovalStatus == "approved"),
            active_bakeries = await _dbContext.Bakeries.CountAsync(),
            total_products = await _dbContext.Products.CountAsync(),
            total_reviews = await _dbContext.Reviews.CountAsync(x => x.ReviewDate >= from && x.ReviewDate <= to)
        };

        ViewData["TopSuppliers"] = await _dbContext.Orders
            .Include(x => x.Supplier)
            .Where(x => x.OrderDate >= from && x.OrderDate <= to)
            .GroupBy(x => new { x.SupplierId, SupplierName = x.Supplier!.SupplierName })
            .Select(x => new { x.Key.SupplierId, x.Key.SupplierName, TotalOrders = x.Count(), TotalRevenue = x.Sum(v => v.TotalAmount) })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(10)
            .ToListAsync();

        return View();
    }
}
