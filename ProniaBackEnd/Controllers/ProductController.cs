using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.Utilities.Exceptions;
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


        public async Task<IActionResult> Detail(int id)
        {

            if (id <= 0) throw new WrongRequestException("The request sent is invalid");
           
            Product product = await _db.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductColors).ThenInclude(pt=>pt.Color)
                .Include(p=>p.ProductSizes).ThenInclude(pt=>pt.Size)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (product == null) throw new NotFoundException("No such product found");

            List<Product> products = await _db.Products
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .ToListAsync();


            DetailVM vm = new DetailVM
            {
                Product = product,
                RelatedProducts = products,
            };

            return View(vm);


            

        }

    }
}
