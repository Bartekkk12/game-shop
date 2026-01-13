using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Kontroler zarządzający katalogiem gier
/// Obsługuje przeglądanie, dodawanie, edycję i usuwanie gier
/// </summary>
public class GamesController : Controller
{
    private readonly GameShopContext _context;

    public GamesController(GameShopContext context)
    {
        _context = context;
    }

    // GET: Games
    // Katalog wszystkich gier z kategoriami i wydawcami
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

    // GET: Games/Details/<id>
    // Szczegóły gry z przyciskami zakupu
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var game = await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (game == null)
        {
            return NotFound();
        }

        return View(game);
    }

    // GET: Games/Create
    // Formularz dodawania nowej gry (tylko Admin)
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name");
        return View();
    }

    // POST: Games/Create
    // Zapisuje nową grę w bazie danych
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Game game)
    {
        ModelState.Remove("Category");
        ModelState.Remove("Publisher");

        if (!ModelState.IsValid)
        {
            Console.WriteLine("Model state is invalid:");
            foreach (var error in ModelState)
            {
                Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", game.PublisherId);
            return View(game);
        }

        Console.WriteLine($"Dodawanie gry: {game.Title}, CategoryId: {game.CategoryId}, PublisherId: {game.PublisherId}, Platform: {game.GamePlatform}");
        _context.Add(game);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Gra {game.Title} została dodana z ID: {game.Id}");
        return RedirectToAction(nameof(Index));
    }

    // GET: Games/Edit/<id>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var game = await _context.Games.FindAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
        ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", game.PublisherId);

        return View(game);
    }

    // POST: Games/Edit/<id>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Game game)
    {
        if (id != game.Id)
        {
            return NotFound();
        }

        ModelState.Remove("Category");
        ModelState.Remove("Publisher");

        if (ModelState.IsValid)
        {
            try
            {
                Console.WriteLine($"Aktualizacja gry: {game.Title}");
                _context.Update(game);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(game.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", game.CategoryId);
        ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", game.PublisherId);

        return View(game);
    }
[Authorize(Roles = "Admin")]

    // GET: Games/Delete/<id>
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var game = await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (game == null)
        {
            return NotFound();
        }

        return View(game);
    }

    // POST: Games/Delete/<id>
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game != null)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Gra o ID {id} została usunięta.");
        }
        return RedirectToAction(nameof(Index));
    }

    private bool GameExists(int id)
    {
        return _context.Games.Any(e => e.Id == id);
    }
}