using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.Areas.Admin.ViewModels;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int page)
        {
            double count = await _db.Categories.CountAsync();

            List<Category> categories = await _db.Categories.Skip(page*4).Take(4).Include(c => c.Products).ToListAsync();

            PaginateVM<Category> paginateVM = new PaginateVM<Category>
            {
                CurrentPage= page,
                TotalPage= Math.Ceiling(count/4),
                Items= categories
            };

            return View(paginateVM);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _db.Categories.Any(c => c.Name.ToLower().Trim() == categoryVM.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir category artiq movcuddur");
                return View();
            }


            Category category = new Category
            {
                Name = categoryVM.Name
            };
            await _db.AddAsync(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //Get Update//
        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();

            UpdateCategoryVM categoryVM = new UpdateCategoryVM
            {
                Name = category.Name
            };

            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            Category existed = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool result = _db.Categories.Any(c => c.Name == categoryVM.Name && c.Id != id);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda category artiq var");
                return View(categoryVM);
            }

            existed.Name = categoryVM.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Category existed = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _db.Categories.Remove(existed);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (category is null) return NotFound();

            return View(category);
        }
    }
}
