using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.Areas.Admin.ViewModels;
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _db.Colors.Include(c=>c.ProductColors).ToListAsync();
            return View(colors);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View(colorVM);
            }

            bool result = _db.Colors.Any(c => c.Name.ToLower().Trim() == colorVM.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Color.Name", "Bu adda reng movcuddur");
                return View(colorVM);
            }

            Color color = new Color
            {
                Name= colorVM.Name
            };

            await _db.Colors.AddAsync(color);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();

            Color color = await _db.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();

            UpdateColorVM colorVM = new UpdateColorVM
            {
                Name = color.Name
            };

            return View(colorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View(colorVM);
            }

            Color existed = await _db.Colors.Include(c=>c.ProductColors).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            bool result = _db.Colors.Any(c => c.Name == colorVM.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda artiq color var");
                return View(colorVM);
            }

            existed.Name = colorVM.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Color existed = await _db.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _db.Colors.Remove(existed);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Color color = _db.Colors.FirstOrDefault(c => c.Id == id);

            if (color is null) return NotFound();

            return View(color);
        }
    }
}
