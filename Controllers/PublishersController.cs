using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class PublishersController : Controller
{
    private readonly GameShopContext _context;

    public PublishersController(GameShopContext context)
    {
        _context = context;
    }

    // GET: Publishers
    public async Task<IActionResult> Index()
    {
        var publishers = await _context.Publishers.ToListAsync();
        return View(publishers);
    }

    // GET: Publishers/Details/<id>
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(m => m.Id == id);

        if (publisher == null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    // GET: Publishers/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Publishers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Publisher publisher)
    {
        if (ModelState.IsValid)
        {
            _context.Add(publisher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(publisher);
    }

    // GET: Publishers/Edit/<id>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    // POST: Publishers/Edit/<id>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Publisher publisher)
    {
        if (id != publisher.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(publisher);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(publisher.Id))
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

        return View(publisher);
    }

    // POST: Publishers/Delete/<id>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(m => m.Id == id);

        if (publisher == null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    // POST: Publishers/Delete/<id>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher != null)
        {
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PublisherExists(int id)
    {
        return _context.Publishers.Any(e => e.Id == id);
    }
}