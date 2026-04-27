using Bake_Link.Data;
using Bake_Link.Infrastructure;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Supplier;

[Area("Supplier")]
[Route("supplier/profile")]
public class ProfileController : Controller
{
    private readonly BakeLinkDbContext _dbContext;
    private readonly PasswordHasher<Models.User> _passwordHasher = new();

    public ProfileController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var supplierId = User.GetRelatedId();
        var supplier = supplierId.HasValue ? await _dbContext.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == supplierId.Value) : null;
        return supplier is null ? Unauthorized() : View(supplier);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(SupplierProfileRequest request)
    {
        var supplierId = User.GetRelatedId();
        var supplier = supplierId.HasValue ? await _dbContext.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == supplierId.Value) : null;
        var user = supplierId.HasValue ? await _dbContext.Users.FirstOrDefaultAsync(x => x.Role == "supplier" && x.RelatedId == supplierId.Value) : null;
        if (supplier is null || user is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        user.Name = request.SupplierName;
        user.Email = request.Email;
        supplier.SupplierName = request.SupplierName;
        supplier.Email = request.Email;
        supplier.PhoneNumber = request.PhoneNumber;
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePassword(PasswordUpdateRequest request)
    {
        var supplierId = User.GetRelatedId();
        var user = supplierId.HasValue ? await _dbContext.Users.FirstOrDefaultAsync(x => x.Role == "supplier" && x.RelatedId == supplierId.Value) : null;
        if (user is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.Password ?? string.Empty, request.CurrentPassword);
        if (verification == PasswordVerificationResult.Failed)
        {
            TempData["Error"] = "Current password is incorrect.";
            return RedirectToAction(nameof(Index));
        }

        user.Password = _passwordHasher.HashPassword(user, request.NewPassword);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Destroy(DeleteAccountRequest request)
    {
        var supplierId = User.GetRelatedId();
        var supplier = supplierId.HasValue ? await _dbContext.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == supplierId.Value) : null;
        var user = supplierId.HasValue ? await _dbContext.Users.FirstOrDefaultAsync(x => x.Role == "supplier" && x.RelatedId == supplierId.Value) : null;
        if (supplier is null || user is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        _dbContext.Suppliers.Remove(supplier);
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
