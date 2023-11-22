using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;

namespace ProniaBackEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _db.Slides.OrderBy(s=>s.Order).Take(2).ToListAsync();

            List<Product> products = await _db.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)).Take(8).ToListAsync();


            HomeVM vm = new HomeVM
            {
                Slides = slides,
                Products = products
            };

            return View(vm);
        }
    }
}
