using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Kontroler zarządzający kategoriami gier (CRUD)
/// Tylko administratorzy mogą dodawać, edytować i usuwać kategorie
/// </summary>
public class CategoriesController : Controller
{
    private readonly GameShopContext _context;

    public CategoriesController(GameShopContext context)
    {
        _context = context;
    }

    // GET: Categories
    // Lista wszystkich kategorii
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

    // GET: Categories/Details/<id>
    // Szczegóły wybranej kategorii
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: Categories/Create
    // Formularz dodawania nowej kategorii (tylko Admin)
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categories/Create
    // Zapisuje nową kategorię w bazie danych
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine($"Dodawanie kategorii: {category.Name}");

            _context.Add(category);
            await _context.SaveChangesAsync();
            Console.WriteLine("Kategoria została dodana do bazy danych.");

            return RedirectToAction(nameof(Index));
        }

        foreach (var state in ModelState)
        {
            Console.WriteLine($"Pole: {state.Key}");
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($" - {error.ErrorMessage}");
            }
        }

        Console.WriteLine("ModelState jest niepoprawny. Kategoria NIE została zapisana.");
        return View(category);
    }

    // GET: Categories/Edit/<id>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Edit/<id>
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

    // GET: Categories/Delete/<id>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Delete/<id>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}