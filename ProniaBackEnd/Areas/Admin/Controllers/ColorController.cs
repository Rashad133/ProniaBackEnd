using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _db;
        public ColorController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _db.Colors.Include(c => c.ProductColors).ToListAsync();
            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View(color);
            }

            bool result = _db.Colors.Any(c => c.Name.ToLower().Trim() == color.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Color.Name", "Bu adda reng movcuddur");
                return View(color);
            }

            await _db.Colors.AddAsync(color);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();

            Color color = await _db.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();

            return View(color);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Color existed = await _db.Colors.FirstOrDefaultAsync(t => t.Id == id);
            if (existed is null) return NotFound();

            bool result = _db.Colors.Any(t => t.Name == color.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda artiq color var");
                return View();
            }

            existed.Name = color.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Color existed = await _db.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _db.Colors.Remove(existed);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Color color = _db.Colors.FirstOrDefault(c => c.Id == id);

            if (color is null) return NotFound();

            return View(color);
        }
    }
}
