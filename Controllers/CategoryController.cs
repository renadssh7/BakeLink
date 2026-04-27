using Bake_Link.Data;
using Bake_Link.Models;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers.Admin;

[Area("Admin")]
[Route("admin/categories")]
public class CategoryController : Controller
{
    private readonly BakeLinkDbContext _dbContext;

    public CategoryController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var categories = await _dbContext.Categories
            .Include(x => x.Products)
            .OrderByDescending(x => x.CategoryId)
            .ToListAsync();

        return View(categories);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Store(CategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        _dbContext.Categories.Add(new Category
        {
            CategoryName = request.CategoryName,
            Description = request.Description,
            Image = request.Image
        });
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, CategoryRequest request)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        if (category is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        category.CategoryName = request.CategoryName;
        category.Description = request.Description;
        category.Image = request.Image;
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Destroy(int id)
    {
        var category = await _dbContext.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.CategoryId == id);
        if (category is null)
        {
            return NotFound();
        }

        if (category.Products.Count > 0)
        {
            TempData["Error"] = "Cannot delete category with associated products.";
            return RedirectToAction(nameof(Index));
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
