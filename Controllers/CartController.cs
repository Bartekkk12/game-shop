using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

/// <summary>
/// Kontroler obsługujący koszyk zakupowy
/// Koszyk to Order ze statusem 'Cart', który jest konwertowany na zamówienie przy kasowaniu
/// </summary>
public class CartController : Controller
{
    private readonly GameShopContext _context;

    public CartController(GameShopContext context)
    {
        _context = context;
    }

    // GET: Cart
    // Wyświetla koszyk użytkownika z produktami
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var cart = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .ThenInclude(g => g.Category)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .ThenInclude(g => g.Publisher)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.status == Order.Status.Cart);

        if (cart == null)
        {
            cart = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                status = Order.Status.Cart
            };
        }

        return View(cart);
    }

    // POST: Cart/AddToCart
    // Dodaje grę do koszyka lub zwiększa ilość jeśli już istnieje
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddToCart(int gameId, int quantity = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var game = await _context.Games.FindAsync(gameId);
        if (game == null)
        {
            return NotFound();
        }

        if (game.Stock < quantity)
        {
            TempData["ErrorMessage"] = $"Dostępne tylko {game.Stock} szt.";
            return RedirectToAction("Index", "Games");
        }

        var cart = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.status == Order.Status.Cart);

        if (cart == null)
        {
            cart = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                status = Order.Status.Cart
            };
            _context.Orders.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = cart.OrderItems.FirstOrDefault(oi => oi.GameId == gameId);
        
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (newQuantity > game.Stock)
            {
                TempData["ErrorMessage"] = $"Maksymalna dostępna ilość: {game.Stock} szt.";
                return RedirectToAction("Index", "Games");
            }
            existingItem.Quantity = newQuantity;
        }
        else
        {
            var orderItem = new OrderItem
            {
                OrderId = cart.Id,
                GameId = gameId,
                Quantity = quantity,
                Price = game.Price
            };
            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Dodano do koszyka!";
        return RedirectToAction("Index", "Games");
    }

    // POST: Cart/UpdateQuantity
    // Aktualizuje ilość produktu w koszyku
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> UpdateQuantity(int orderItemId, int quantity)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var orderItem = await _context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Game)
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.Order.UserId == userId && oi.Order.status == Order.Status.Cart);

        if (orderItem == null)
        {
            return NotFound();
        }

        if (quantity <= 0)
        {
            _context.OrderItems.Remove(orderItem);
        }
        else if (quantity > orderItem.Game.Stock)
        {
            TempData["ErrorMessage"] = $"Dostępne tylko {orderItem.Game.Stock} szt.";
        }
        else
        {
            orderItem.Quantity = quantity;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: Cart/RemoveItem
    // Usuwa produkt z koszyka
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> RemoveItem(int orderItemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var orderItem = await _context.OrderItems
            .Include(oi => oi.Order)
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.Order.UserId == userId && oi.Order.status == Order.Status.Cart);

        if (orderItem == null)
        {
            return NotFound();
        }

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Usunięto z koszyka";
        return RedirectToAction(nameof(Index));
    }

    // POST: Cart/Checkout
    // Tworzy zamówienie z koszyka, zmniejsza stan magazynowy i usuwa koszyk
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var cart = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.status == Order.Status.Cart);

        if (cart == null || !cart.OrderItems.Any())
        {
            TempData["ErrorMessage"] = "Koszyk jest pusty";
            return RedirectToAction(nameof(Index));
        }

        // Sprawdź dostępność wszystkich produktów
        foreach (var item in cart.OrderItems)
        {
            if (item.Game.Stock < item.Quantity)
            {
                TempData["ErrorMessage"] = $"Niewystarczająca ilość: {item.Game.Title}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Utwórz nowe zamówienie
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            status = Order.Status.New
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Przenieś items z koszyka do zamówienia i zmniejsz stan magazynowy
        foreach (var cartItem in cart.OrderItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                GameId = cartItem.GameId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price
            };

            _context.OrderItems.Add(orderItem);
            
            cartItem.Game.Stock -= cartItem.Quantity;
            _context.Games.Update(cartItem.Game);
        }

        // Usuń koszyk
        _context.OrderItems.RemoveRange(cart.OrderItems);
        _context.Orders.Remove(cart);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Zamówienie zostało utworzone!";
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }

    // POST: Cart/Clear
    // Usuwa wszystkie produkty z koszyka
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Clear()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var cart = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.status == Order.Status.Cart);

        if (cart != null)
        {
            _context.OrderItems.RemoveRange(cart.OrderItems);
            _context.Orders.Remove(cart);
            await _context.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "Koszyk został wyczyszczony";
        return RedirectToAction(nameof(Index));
    }
}
