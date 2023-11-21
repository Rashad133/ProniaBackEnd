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
        public IActionResult Index()
        {
            List<Slide> slides = _db.Slides.OrderBy(s=>s.Order).Take(2).ToList();

            List<Product> products = _db.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)).Take(8).ToList();

            List<Product> product = _db.Products.Include(p=>p.ProductImages).Take(8).ToList();


            HomeVM vm = new HomeVM
            {
                Slides = slides,
                Products = products
            };

            return View(vm);
        }
    }
}
