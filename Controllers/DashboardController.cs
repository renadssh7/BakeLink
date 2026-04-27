using Bake_Link.Constants;
using Bake_Link.Data;
using Bake_Link.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Supplier;

[Area("Supplier")]
[Route("supplier/dashboard")]
public class DashboardController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public DashboardController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var supplierId = User.GetRelatedId();
        if (!supplierId.HasValue)
        {
            return Unauthorized();
        }

        ViewData["Stats"] = new
        {
            total_products = await _dbContext.Products.CountAsync(x => x.SupplierId == supplierId.Value),
            pending_orders = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && (x.OrderStatus == OrderStatuses.Pending || x.OrderStatus == OrderStatuses.Confirmed)),
            completed_orders = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Delivered),
            total_revenue = await _dbContext.Orders.Where(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Delivered).SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            average_rating = await _dbContext.Reviews.Where(x => x.SupplierId == supplierId.Value).AverageAsync(x => (double?)x.Rate) ?? 0,
            total_reviews = await _dbContext.Reviews.CountAsync(x => x.SupplierId == supplierId.Value)
        };

        ViewData["RecentOrders"] = await _dbContext.Orders
            .Include(x => x.Bakery)
            .Where(x => x.SupplierId == supplierId.Value)
            .OrderByDescending(x => x.OrderDate)
            .Take(5)
            .ToListAsync();

        ViewData["TopProducts"] = await _dbContext.Products
            .Where(x => x.SupplierId == supplierId.Value)
            .OrderByDescending(x => x.ProductId)
            .Take(5)
            .ToListAsync();

        ViewData["RecentReviews"] = await _dbContext.Reviews
            .Include(x => x.Bakery)
            .Where(x => x.SupplierId == supplierId.Value)
            .OrderByDescending(x => x.ReviewDate)
            .Take(5)
            .ToListAsync();

        return View();
    }
}
