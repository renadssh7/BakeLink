using Bake_Link.Data;
using Bake_Link.Infrastructure;
using Bake_Link.Models;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Supplier;

[Area("Supplier")]
[Route("supplier/addresses")]
public class AddressController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public AddressController(BakeLinkDbContext dbContext)
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

        var addresses = await _dbContext.Addresses
            .Where(x => x.SupplierId == supplierId.Value)
            .ToListAsync();

        return View(addresses);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Store(AddressRequest request)
    {
        var supplierId = User.GetRelatedId();
        if (!supplierId.HasValue)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        _dbContext.Addresses.Add(new Address
        {
            SupplierId = supplierId.Value,
            City = request.City,
            District = request.District,
            Street = request.Street,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        });

        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var supplierId = User.GetRelatedId();
        var address = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressId == id && x.SupplierId == supplierId);
        return address is null ? NotFound() : View("Edit", address);
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, AddressRequest request)
    {
        var supplierId = User.GetRelatedId();
        var address = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressId == id && x.SupplierId == supplierId);
        if (address is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("Edit", address);
        }

        address.City = request.City;
        address.District = request.District;
        address.Street = request.Street;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Destroy(int id)
    {
        var supplierId = User.GetRelatedId();
        var address = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressId == id && x.SupplierId == supplierId);
        if (address is null)
        {
            return NotFound();
        }

        _dbContext.Addresses.Remove(address);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
