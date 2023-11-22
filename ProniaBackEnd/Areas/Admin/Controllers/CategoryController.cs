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


            //Office -- office
            //office -- office

            bool result = _db.Categories.Any(c=>c.Name.ToLower().Trim()==category.Name.ToLower().Trim());

            if(result)
            {
                ModelState.AddModelError("Name", "Bele bir category artiq movcuddur");
                return View();
            }


            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
