using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        public ProductController(AppDbContext db)
        {
            _db = db; 
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}


        public IActionResult Detail(int id)
        {
        

            if(id <= 0) return BadRequest();
           

            Product product = _db.Products.Include(p=>p.Category).FirstOrDefault(x => x.Id == id);

            List<Product> products = _db.Products.Where(p=>p.CategoryId==product.CategoryId).ToList();

            if(product == null) return NotFound();

            return View(product);
        }

    }
}
