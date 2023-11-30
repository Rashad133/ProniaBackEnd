using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;
        public ProductViewComponent(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(int key=1)
        {
            List<Product> products;

            switch (key)
            {
                case 1:
                    products = await _db.Products.OrderBy(p=>p.Name).Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break; 
                case 2:
                    products = await _db.Products.OrderByDescending(p=>p.Price).Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;
                case 3:
                    products = await _db.Products.OrderByDescending(p=>p.Id).Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;
                default:
                    products = await _db.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;
            }
            return View(products);
            
        }
    }
}
