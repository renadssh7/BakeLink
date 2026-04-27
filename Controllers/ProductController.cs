using Bake_Link.Data;
using Bake_Link.Infrastructure;
using Bake_Link.Models;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Supplier;

[Area("Supplier")]
[Route("supplier/products")]
public class ProductController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public ProductController(BakeLinkDbContext dbContext)
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

        ViewData["Categories"] = await _dbContext.Categories.OrderBy(x => x.CategoryName).ToListAsync();

        var products = await _dbContext.Products
            .Include(x => x.Category)
            .Where(x => x.SupplierId == supplierId.Value)
            .OrderByDescending(x => x.ProductId)
            .ToListAsync();

        return View(products);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Store(ProductRequest request)
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

        _dbContext.Products.Add(new Product
        {
            SupplierId = supplierId.Value,
            CategoryId = request.CategoryId,
            ProductName = request.ProductName,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Images = ParseImages(request.Images)
        });

        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var supplierId = User.GetRelatedId();
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.SupplierId == supplierId);
        return product is null ? NotFound() : Json(product);
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, ProductRequest request)
    {
        var supplierId = User.GetRelatedId();
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.SupplierId == supplierId);
        if (product is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        var images = request.ReplaceImages ? new List<string>() : product.Images;
        images.AddRange(ParseImages(request.Images));

        product.CategoryId = request.CategoryId;
        product.ProductName = request.ProductName;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.Images = images;

        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Destroy(int id)
    {
        var supplierId = User.GetRelatedId();
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.SupplierId == supplierId);
        if (product is null)
        {
            return NotFound();
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private static List<string> ParseImages(string? rawImages)
    {
        return string.IsNullOrWhiteSpace(rawImages)
            ? []
            : rawImages
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => Uri.IsWellFormedUriString(x, UriKind.Absolute))
                .ToList();
    }
}
