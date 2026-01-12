using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class CategoriesController : Controller
{
    private readonly GameShopContext _context;

    public CategoriesController(GameShopContext context)
    {
        _context = context;
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.ToListAsync();
        Console.WriteLine($"Liczba kategorii w bazie: {categories.Count}");
        foreach (var category in categories)
        {
            Console.WriteLine($"Kategoria: {category.Name}");
        }
        return View(categories);
    }

    // GET: Categories/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Wyszukanie kategorii po id
        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null) // Nie znaleziono kategorii
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: Categories/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine($"Dodawanie kategorii: {category.Name}");

            _context.Add(category); // Dodanie obiektu do kontekstu
            await _context.SaveChangesAsync(); // Zapisanie zmian do bazy danych
            Console.WriteLine("Kategoria została dodana do bazy danych.");

            return RedirectToAction(nameof(Index)); // Przekierowanie na stronę główną
        }

        // Diagnostic - Wyświetlenie wszystkich błędów walidacji:
        foreach (var state in ModelState)
        {
            Console.WriteLine($"Pole: {state.Key}");
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($" - {error.ErrorMessage}");
            }
        }

        Console.WriteLine("ModelState jest niepoprawny. Kategoria NIE została zapisana.");
        return View(category); // Wróć na stronę utworzenia
    }

    // GET: Categories/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Wyszukanie kategorii po id
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
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

        return View(category);
    }

    // GET: Categories/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) // Brak id dla zapytania usunięcia
        {
            return NotFound();
        }

        // Pobranie kategorii dla potwierdzenia usunięcia
        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null) // Kategoria nie istnieje
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id); // Wyszukanie encji
        if (category != null)
        {
            _context.Categories.Remove(category); // Usunięcie z bazy
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // Sprawdzanie istnienia kategorii
    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}