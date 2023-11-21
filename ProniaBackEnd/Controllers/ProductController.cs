using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;


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
           
            Product product = _db.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductColors).ThenInclude(pt=>pt.Color)
                .Include(p=>p.ProductSizes).ThenInclude(pt=>pt.Size)
                .FirstOrDefault(x => x.Id == id);


            if(product == null) return NotFound();

            List<Product> products = _db.Products
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .ToList();


            DetailVM vm = new DetailVM
            {
                Product = product,
                RelatedProducts = products,
            };

            return View(vm);


            

        }

    }
}
