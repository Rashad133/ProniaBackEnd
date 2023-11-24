using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories= await _db.Categories.Include(c=>c.Products).ToListAsync(); 

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            
            bool result = _db.Categories.Any(c=>c.Name.ToLower().Trim()==category.Name.ToLower().Trim());

            if(result)
            {
                ModelState.AddModelError("Name", "Bele bir category artiq movcuddur");
                return View();
            }


            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //Get Update//
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if(category is null) return NotFound();

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Category existed = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool result = _db.Categories.Any(c=>c.Name==category.Name && c.Id!=id);

            if (result)
            {   
                ModelState.AddModelError("Name", "Bu adda category artiq var");
                return View();
            }

            existed.Name = category.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));   

        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Category existed = await _db.Categories.FirstOrDefaultAsync(c=>c.Id == id);

            if(existed is null) return NotFound();

            _db.Categories.Remove(existed);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (category is null) return NotFound();

            return View(category);
        }
    }
}
