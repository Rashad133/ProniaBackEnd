using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.Areas.Admin.ViewModels;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        public ProductController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products=await _db.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages
                .Where(pi=>pi.IsPrimary==true))
                .ToListAsync();


            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories=await _db.Categories.ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                return View();
            }
            bool result = await _db.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);
            if (!result)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                ModelState.AddModelError("CategoryId","Bu Id-li category yoxdu");
                return View();
            }


            Product product = new Product
            {
                Name= productVM.Name,
                Price= productVM.Price,
                SKU=productVM.SKU,
                CategoryId=productVM.CategoryId,
                Description=productVM.Description,
                Image=productVM.Image

            };

            await _db.Products.AddAsync(product);
            await _db.AddRangeAsync();

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = _db.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductColors)
                .ThenInclude(p=>p.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(p=>p.Size)
                .Include(p=>p.ProductTags)
                .ThenInclude(p=>p.Tag)
                .FirstOrDefault(c => c.Id == id);

            if (product is null) return NotFound();

            return View(product );
        }

        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();

            Product existed = await _db.Products.FirstOrDefaultAsync(p=> p.Id == id);

            if(existed is null) return NotFound();

            _db.Products.Remove(existed);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
