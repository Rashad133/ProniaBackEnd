using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;

namespace ProniaBackEnd.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _db;
        public ShopController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(string search,int? order=1)
        {
            IQueryable<Product> query = _db.Products.Include(p => p.ProductImages).AsQueryable();

            switch (order)
            {
                case 1:
                    query = query.OrderBy(p => p.Name);
                    break;
                case 2:
                    query = query.OrderBy(p => p.Price);
                    break;
                case 3:
                    query = query.OrderByDescending(p => p.Id);
                    break;
            }

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(p=>p.Name.ToLower().Contains(search.ToLower()));
            }
            ShopVM shopVM = new ShopVM
            {
                Categories = await _db.Categories.Include(c=>c.Products).ToListAsync(),
                Products = await _db.Products.Include(p=>p.ProductImages).ToListAsync()
            };
            return View(shopVM);
        }
    }
}
