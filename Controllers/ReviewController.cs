using Bake_Link.Data;
using Bake_Link.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Supplier;

[Area("Supplier")]
[Route("supplier/reviews")]
public class ReviewController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public ReviewController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] int? rating)
    {
        var supplierId = User.GetRelatedId();
        if (!supplierId.HasValue)
        {
            return Unauthorized();
        }

        var query = _dbContext.Reviews
            .Include(x => x.Bakery)
            .Where(x => x.SupplierId == supplierId.Value)
            .AsQueryable();

        if (rating.HasValue)
        {
            query = query.Where(x => x.Rate == rating.Value);
        }

        ViewData["Stats"] = new
        {
            total = await _dbContext.Reviews.CountAsync(x => x.SupplierId == supplierId.Value),
            average = await _dbContext.Reviews.Where(x => x.SupplierId == supplierId.Value).AverageAsync(x => (double?)x.Rate) ?? 0
        };

        return View(await query.OrderByDescending(x => x.ReviewDate).ToListAsync());
    }
}
