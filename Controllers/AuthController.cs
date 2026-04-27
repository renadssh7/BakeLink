using Bake_Link.Data;
using Bake_Link.Models;
using Bake_Link.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly BakeLinkDbContext _dbContext;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthController(BakeLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null)
        {
            ModelState.AddModelError(nameof(request.Email), "This email does not exist in our records.");
            return View(request);
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.Password ?? string.Empty, request.Password);
        if (verification == PasswordVerificationResult.Failed && user.Password != request.Password)
        {
            ModelState.AddModelError(nameof(request.Password), "The provided password is incorrect.");
            return View(request);
        }

        TempData["Info"] = "Authentication wiring is still pending. Cookie sign-in still needs ASP.NET Core Identity or custom auth setup.";
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        if (await _dbContext.Users.AnyAsync(x => x.Email == request.Email))
        {
            ModelState.AddModelError(nameof(request.Email), "This email is already registered.");
            return View(request);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            int relatedId;

            if (request.Role == "bakery")
            {
                var bakery = new Bakery
                {
                    BakeryName = request.BusinessName,
                    Email = request.Email,
                    Password = string.Empty,
                    PhoneNumber = request.Phone
                };
                _dbContext.Bakeries.Add(bakery);
                await _dbContext.SaveChangesAsync();

                _dbContext.Addresses.Add(new Address
                {
                    BakerId = bakery.BakeryId,
                    City = request.City,
                    District = request.District,
                    Street = request.Street
                });

                relatedId = bakery.BakeryId;
            }
            else
            {
                var supplier = new Models.Supplier
                {
                    SupplierName = request.BusinessName,
                    Email = request.Email,
                    Password = string.Empty,
                    PhoneNumber = request.Phone,
                    ApprovalStatus = "pending"
                };
                _dbContext.Suppliers.Add(supplier);
                await _dbContext.SaveChangesAsync();

                _dbContext.Addresses.Add(new Address
                {
                    SupplierId = supplier.SupplierId,
                    City = request.City,
                    District = request.District,
                    Street = request.Street
                });

                relatedId = supplier.SupplierId;
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Role = request.Role,
                RelatedId = relatedId
            };
            user.Password = _passwordHasher.HashPassword(user, request.Password);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Registration completed. Authentication sign-in still needs ASP.NET Core auth wiring.";
            return RedirectToAction(nameof(Login));
        }
        catch
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            return View(request);
        }
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
