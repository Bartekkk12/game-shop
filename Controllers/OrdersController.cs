using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class OrdersController : Controller
{
    private readonly GameShopContext _context;

    public OrdersController(GameShopContext context)
    {
        _context = context;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        IQueryable<Order> ordersQuery = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .Include(o => o.User);

        if (User.IsInRole("User") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ordersQuery = ordersQuery.Where(o => o.UserId == userId);
        }

        var orders = await ordersQuery.OrderByDescending(o => o.OrderDate).ToListAsync();

        return View(orders);
    }

    // GET: Orders/Details/<id>
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .ThenInclude(g => g.Category)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .ThenInclude(g => g.Publisher)
            .Include(o => o.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId)
            {
                return Forbid();
            }
        }

        return View(order);
    }

    // GET: Orders/Create
    [Authorize]
    public IActionResult Create()
    {
        var games = _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .Where(g => g.Stock > 0)
            .ToList();

        ViewBag.Games = games;
        return View();
    }

    // POST: Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(int[] gameIds, int[] quantities)
    {
        if (gameIds == null || gameIds.Length == 0)
        {
            ModelState.AddModelError("", "Musisz wybrać przynajmniej jedną grę");
            var games = _context.Games
                .Include(g => g.Category)
                .Include(g => g.Publisher)
                .Where(g => g.Stock > 0)
                .ToList();
            ViewBag.Games = games;
            return View();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            status = Order.Status.New
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        for (int i = 0; i < gameIds.Length; i++)
        {
            var game = await _context.Games.FindAsync(gameIds[i]);
            if (game != null && quantities[i] > 0)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    GameId = gameIds[i],
                    Quantity = quantities[i],
                    Price = game.Price
                };

                _context.OrderItems.Add(orderItem);
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"Zamówienie {order.Id} zostało utworzone z {gameIds.Length} pozycjami przez użytkownika {userId}");

        return RedirectToAction(nameof(Details), new { id = order.Id });
    }

    // GET: Orders/Edit/<id>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: Orders/Edit/<id>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Order.Status status)
    {
        var order = await _context.Orders.FindAsync(id);
        
        if (order == null)
        {
            return NotFound();
        }

        order.status = status;
        _context.Update(order);
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"Status zamówienia {id} został zmieniony na {status}");
        return RedirectToAction(nameof(Details), new { id = id });
    }

    // GET: Orders/Delete/<id>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: Orders/Delete/<id>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null)
        {
            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Zamówienie {id} zostało usunięte");
        }

        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
