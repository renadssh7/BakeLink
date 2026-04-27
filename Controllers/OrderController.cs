using Bake_Link.Constants;
using Bake_Link.Data;
using Bake_Link.Infrastructure;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Supplier;

[Area("Supplier")]
[Route("supplier/orders")]
public class OrderController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public OrderController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string? status, [FromQuery] string? search)
    {
        var supplierId = User.GetRelatedId();
        if (!supplierId.HasValue)
        {
            return Unauthorized();
        }

        var query = _dbContext.Orders
            .Include(x => x.Bakery)
            .Where(x => x.SupplierId == supplierId.Value)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && status != "all")
        {
            query = query.Where(x => x.OrderStatus == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.OrderId.ToString().Contains(search) ||
                (x.Bakery != null && x.Bakery.BakeryName != null && x.Bakery.BakeryName.Contains(search)));
        }

        ViewData["Counts"] = new
        {
            pending = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Pending),
            confirmed = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Confirmed),
            preparing = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Preparing),
            shipped = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Shipped),
            delivered = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Delivered),
            cancelled = await _dbContext.Orders.CountAsync(x => x.SupplierId == supplierId.Value && x.OrderStatus == OrderStatuses.Cancelled)
        };
        ViewData["Statuses"] = OrderStatuses.All;

        return View(await query.OrderByDescending(x => x.OrderDate).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Show(int id)
    {
        var supplierId = User.GetRelatedId();
        var order = await _dbContext.Orders
            .Include(x => x.Bakery)
            .Include(x => x.Address)
            .Include(x => x.Delivery)
            .FirstOrDefaultAsync(x => x.OrderId == id && x.SupplierId == supplierId);

        return order is null ? NotFound() : Json(order);
    }

    [HttpPost("{id:int}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatusRequest request)
    {
        var supplierId = User.GetRelatedId();
        var order = await _dbContext.Orders.Include(x => x.Delivery).FirstOrDefaultAsync(x => x.OrderId == id && x.SupplierId == supplierId);
        if (order is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid || !OrderStatuses.All.Contains(request.OrderStatus))
        {
            TempData["Error"] = "Invalid order status.";
            return RedirectToAction(nameof(Index));
        }

        order.OrderStatus = request.OrderStatus;
        if (request.OrderStatus == OrderStatuses.Delivered && order.Delivery is not null)
        {
            order.Delivery.DeliveryStatus = DeliveryStatuses.Delivered;
            order.Delivery.DeliveryDate = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
