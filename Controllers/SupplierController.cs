using Bake_Link.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Admin;

[Area("Admin")]
[Route("admin/suppliers")]
public class SupplierController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public SupplierController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string? status, [FromQuery] string? search)
    {
        var query = _dbContext.Suppliers
            .Include(x => x.Products)
            .Include(x => x.Orders)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && status != "all")
        {
            query = query.Where(x => x.ApprovalStatus == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                (x.SupplierName != null && x.SupplierName.Contains(search)) ||
                (x.Email != null && x.Email.Contains(search)) ||
                (x.PhoneNumber != null && x.PhoneNumber.Contains(search)));
        }

        ViewData["Summary"] = new
        {
            totalSuppliers = await _dbContext.Suppliers.CountAsync(),
            pendingCount = await _dbContext.Suppliers.CountAsync(x => x.ApprovalStatus == "pending"),
            approvedCount = await _dbContext.Suppliers.CountAsync(x => x.ApprovalStatus == "approved"),
            rejectedCount = await _dbContext.Suppliers.CountAsync(x => x.ApprovalStatus == "rejected")
        };

        return View(await query.OrderByDescending(x => x.SupplierId).ToListAsync());
    }

    [HttpGet("pending")]
    public async Task<IActionResult> Pending()
    {
        var suppliers = await _dbContext.Suppliers
            .Where(x => x.ApprovalStatus == "pending")
            .OrderByDescending(x => x.SupplierId)
            .ToListAsync();

        return View(suppliers);
    }

    [HttpPost("{id:int}/approve")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);
        if (supplier is null)
        {
            return NotFound();
        }

        supplier.ApprovalStatus = "approved";
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Pending));
    }

    [HttpPost("{id:int}/reject")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);
        if (supplier is null)
        {
            return NotFound();
        }

        supplier.ApprovalStatus = "rejected";
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Pending));
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Destroy(int id)
    {
        var supplier = await _dbContext.Suppliers.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.SupplierId == id);
        if (supplier is null)
        {
            return NotFound();
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Role == "supplier" && x.RelatedId == id);
        if (user is not null)
        {
            _dbContext.Users.Remove(user);
        }

        _dbContext.Addresses.RemoveRange(supplier.Addresses);
        _dbContext.Suppliers.Remove(supplier);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
