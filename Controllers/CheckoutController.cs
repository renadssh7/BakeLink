using System.Text.Json;
using Bake_Link.Constants;
using Bake_Link.Data;
using Bake_Link.Infrastructure;
using Bake_Link.Models;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers;

[Route("checkout")]
public class CheckoutController : Controller
{
    private const string CartSessionKey = "cart";
    private readonly BakeLinkDbContext _dbContext;

    public CheckoutController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var cart = LoadCart();
        if (cart.Count == 0)
        {
            return RedirectToAction(nameof(CartController.Index), "Cart");
        }

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
                var subtotal = x.Quantity * x.Price;
                return new
                {
                    product_id = x.ProductId,
                    name = x.Name,
                    quantity = x.Quantity,
                    price = x.Price,
                    subtotal,
                    supplier_name = product.Supplier?.SupplierName,
                    supplier_id = product.SupplierId,
                    image = x.Image
                };
            })
            .ToList();

        var subtotalAmount = cartItems.Sum(x => x.subtotal);
        var totalItems = cartItems.Sum(x => x.quantity);
        const decimal delivery = 45.00m;
        var tax = subtotalAmount * 0.15m;
        var total = subtotalAmount + delivery + tax;

        var bakeryId = User.GetRelatedId();
        var addresses = bakeryId.HasValue
            ? await _dbContext.Addresses.Where(x => x.BakerId == bakeryId.Value).ToListAsync()
            : new List<Address>();

        ViewData["Subtotal"] = subtotalAmount;
        ViewData["TotalItems"] = totalItems;
        ViewData["Delivery"] = delivery;
        ViewData["Tax"] = tax;
        ViewData["Total"] = total;
        ViewData["Addresses"] = addresses;

        return View(cartItems);
    }

    [HttpPost("process")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(CheckoutRequest request)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        var bakeryId = User.GetRelatedId();
        if (!bakeryId.HasValue)
        {
            return Unauthorized();
        }

        var cart = LoadCart();
        if (cart.Count == 0)
        {
            return RedirectToAction(nameof(CartController.Index), "Cart");
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            int addressId;
            if (request.NewAddress && !string.IsNullOrWhiteSpace(request.City) && !string.IsNullOrWhiteSpace(request.District) && !string.IsNullOrWhiteSpace(request.Street))
            {
                var address = new Address
                {
                    BakerId = bakeryId.Value,
                    City = request.City,
                    District = request.District,
                    Street = request.Street,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude
                };
                _dbContext.Addresses.Add(address);
                await _dbContext.SaveChangesAsync();
                addressId = address.AddressId;
            }
            else if (request.AddressId.HasValue)
            {
                addressId = request.AddressId.Value;
            }
            else
            {
                TempData["Error"] = "Please select or add a delivery address.";
                return RedirectToAction(nameof(Index));
            }

            var productIds = cart.Select(x => x.ProductId).ToList();
            var products = await _dbContext.Products
                .Include(x => x.Supplier)
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync();

            var grouped = products
                .Join(cart, product => product.ProductId, item => item.ProductId, (product, item) => new { product, item })
                .GroupBy(x => x.product.SupplierId);

            var orderIds = new List<int>();
            foreach (var group in grouped)
            {
                var groupTotal = group.Sum(x => x.item.Price * x.item.Quantity);
                var order = new Order
                {
                    BakeryId = bakeryId.Value,
                    SupplierId = group.Key,
                    AddressId = addressId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = groupTotal,
                    OrderStatus = OrderStatuses.Pending,
                    EstimatedDeliveryCost = 45.00m
                };
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();
                orderIds.Add(order.OrderId);

                var cartEntity = new Cart
                {
                    OrderId = order.OrderId,
                    Quantity = group.Sum(x => x.item.Quantity),
                    UnitPrice = group.Any() ? groupTotal / group.Count() : 0,
                    Subtotal = groupTotal
                };
                _dbContext.Carts.Add(cartEntity);
                await _dbContext.SaveChangesAsync();

                foreach (var line in group)
                {
                    _dbContext.Items.Add(new Item
                    {
                        CartId = cartEntity.CartId,
                        ProductId = line.product.ProductId,
                        Quantity = line.item.Quantity,
                        UnitPrice = line.item.Price,
                        TotalAmount = line.item.Price * line.item.Quantity
                    });
                }

                _dbContext.Deliveries.Add(new Delivery
                {
                    OrderId = order.OrderId,
                    DeliveryStatus = DeliveryStatuses.Waiting,
                    DeliveryCost = 45.00m
                });
            }

            var subtotalAmount = grouped.Sum(x => x.Sum(v => v.item.Price * v.item.Quantity));
            var deliveryAmount = 45.00m * grouped.Count();
            var tax = subtotalAmount * 0.15m;
            var total = subtotalAmount + deliveryAmount + tax;

            var payment = new Payment
            {
                OrderId = orderIds.First(),
                Subtotal = subtotalAmount,
                Tax = tax,
                TotalAmount = total,
                PaymentMethod = "card",
                CardNumber = request.CardNumber[^4..],
                CardHolderName = request.CardHolder,
                CreatedAt = DateTime.UtcNow,
                RefNumber = $"PAY-{Guid.NewGuid():N}"[..16].ToUpperInvariant()
            };
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            var invoice = new Invoice
            {
                PaymentId = payment.PaymentId,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{payment.PaymentId:D5}",
                TotalAmount = total
            };
            _dbContext.Invoices.Add(invoice);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(Success), new { invoiceId = invoice.InvoiceId });
        }
        catch
        {
            await transaction.RollbackAsync();
            TempData["Error"] = "Checkout failed. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("success/{invoiceId:int}")]
    public async Task<IActionResult> Success(int invoiceId)
    {
        var invoice = await _dbContext.Invoices
            .Include(x => x.Payment)
            .ThenInclude(x => x!.Order)
            .FirstOrDefaultAsync(x => x.InvoiceId == invoiceId);

        return invoice is null ? NotFound() : View(invoice);
    }

    private List<CartSessionItem> LoadCart()
    {
        var raw = HttpContext.Session.GetString(CartSessionKey);
        return string.IsNullOrWhiteSpace(raw)
            ? []
            : JsonSerializer.Deserialize<List<CartSessionItem>>(raw) ?? [];
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
