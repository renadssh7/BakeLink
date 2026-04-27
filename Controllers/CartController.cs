using System.Text.Json;
using Bake_Link.Data;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "cart";
    private readonly BakeLinkDbContext _dbContext;

    public CartController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = LoadCart();
        var productIds = cart.Select(x => x.ProductId).ToList();
        var products = await _dbContext.Products
            .Include(x => x.Supplier)
            .Where(x => productIds.Contains(x.ProductId))
            .ToDictionaryAsync(x => x.ProductId);

        var cartItems = cart
            .Where(x => products.ContainsKey(x.ProductId))
            .Select(x =>
            {
                var product = products[x.ProductId];
                return new
                {
                    x.ProductId,
                    x.Name,
                    x.Price,
                    x.Quantity,
                    x.Image,
                    Product = product,
                    Subtotal = x.Price * x.Quantity
                };
            })
            .ToList();

        ViewData["Total"] = cartItems.Sum(x => x.Subtotal);
        return View(cartItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CartItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == request.ProductId);
        if (product is null)
        {
            return NotFound();
        }

        var cart = LoadCart();
        var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
        if (existing is null)
        {
            cart.Add(new CartSessionItem
            {
                ProductId = product.ProductId,
                Name = product.ProductName ?? string.Empty,
                Price = product.Price ?? 0,
                Quantity = request.Quantity,
                Image = product.Images.FirstOrDefault()
            });
        }
        else
        {
            existing.Quantity += request.Quantity;
        }

        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(CartItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cart = LoadCart();
        var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Quantity = request.Quantity;
        SaveCart(cart);

        return Json(new
        {
            success = true,
            message = "Cart updated",
            cart_count = cart.Sum(x => x.Quantity),
            cart_total = cart.Sum(x => x.Price * x.Quantity),
            item_subtotal = existing.Price * existing.Quantity
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        var cart = LoadCart();
        cart.RemoveAll(x => x.ProductId == id);
        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction(nameof(Index));
    }

    private List<CartSessionItem> LoadCart()
    {
        var raw = HttpContext.Session.GetString(CartSessionKey);
        return string.IsNullOrWhiteSpace(raw)
            ? []
            : JsonSerializer.Deserialize<List<CartSessionItem>>(raw) ?? [];
    }

    private void SaveCart(List<CartSessionItem> cart)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
    }

    private class CartSessionItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
    }
}
