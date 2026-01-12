using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class GamesController : Controller
{
    private readonly GameShopContext _context;

    public GamesController(GameShopContext context)
    {
        _context = context;
    }

    // GET: Games
    public async Task<IActionResult> Index()
    {
        var games = await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .ToListAsync();

        Console.WriteLine($"Liczba gier w bazie: {games.Count}");
        foreach (var game in games)
        {
            Console.WriteLine($"Gra: {game.Title}, Kategoria: {game.Category?.Name}, Wydawca: {game.Publisher?.Name}");
        }

        return View(games);
    }

    // GET: Games/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound(); // Brak ID - zwróć widok NotFound
        }

        var game = await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (game == null)
        {
            return NotFound(); // Gra nie została znaleziona
        }

        return View(game);
    }

    // GET: Games/Create
    public IActionResult Create()
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name");
        return View();
    }

    // POST: Games/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Game game)
    {
        if (!ModelState.IsValid)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", game.PublisherId);
            return View(game);
        }

        _context.Add(game);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Games/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound(); // Jeśli nie ma ID - zwróć NotFound
        }

        var game = await _context.Games.FindAsync(id); // Znalezienie gry
        if (game == null)
        {
            return NotFound();
        }

        // Załaduj dane dla pól rozwijanych
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
        ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", game.PublisherId);

        return View(game);
    }

    // POST: Games/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Game game)
    {
        if (id != game.Id)
        {
            return NotFound(); // ID dla gry się nie pokrywa
        }

        if (ModelState.IsValid)
        {
            try
            {
                Console.WriteLine($"Aktualizacja gry: {game.Title}");
                _context.Update(game); // Aktualizacja danych gry w bazie
                await _context.SaveChangesAsync(); // Zapisz zmiany w bazie
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(game.Id))
                {
                    return NotFound(); // Gra już nie istnieje w bazie
                }
                else
                {
                    throw; // Rzuć wyjątek dalej
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // Jeśli walidacja ModelState się nie powiedzie, załaduj ponownie listy rozwijane
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
        ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", game.PublisherId);

        return View(game);
    }

    // GET: Games/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound(); // Brak ID gry
        }

        var game = await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (game == null)
        {
            return NotFound(); // Gra nie została znaleziona
        }

        return View(game);
    }

    // POST: Games/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var game = await _context.Games.FindAsync(id); // Pobieranie gry z bazy
        if (game != null)
        {
            _context.Games.Remove(game); // Usunięcie gry
            await _context.SaveChangesAsync(); // Potwierdzenie usunięcia w bazie
            Console.WriteLine($"Gra o ID {id} została usunięta.");
        }
        return RedirectToAction(nameof(Index));
    }

    private bool GameExists(int id)
    {
        return _context.Games.Any(e => e.Id == id); // Sprawdzenie istnienia gry
    }
}