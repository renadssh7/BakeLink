using Bake_Link.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Admin;

[Area("Admin")]
[Route("admin/bakeries")]
public class BakeryController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public BakeryController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string? search)
    {
        var query = _dbContext.Bakeries
            .Include(x => x.Orders)
            .Include(x => x.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                (x.BakeryName != null && x.BakeryName.Contains(search)) ||
                (x.Email != null && x.Email.Contains(search)) ||
                (x.PhoneNumber != null && x.PhoneNumber.Contains(search)));
        }

        ViewData["Summary"] = new
        {
            totalBakeries = await _dbContext.Bakeries.CountAsync(),
            totalOrders = await _dbContext.Orders.CountAsync(),
            totalReviews = await _dbContext.Reviews.CountAsync()
        };

        return View(await query.OrderByDescending(x => x.BakeryId).ToListAsync());
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Destroy(int id)
    {
        var bakery = await _dbContext.Bakeries.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.BakeryId == id);
        if (bakery is null)
        {
            return NotFound();
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Role == "bakery" && x.RelatedId == id);
        if (user is not null)
        {
            _dbContext.Users.Remove(user);
        }

        _dbContext.Addresses.RemoveRange(bakery.Addresses);
        _dbContext.Bakeries.Remove(bakery);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
