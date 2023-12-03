using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;

namespace ProniaBackEnd.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _db;
        public BasketController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM= new List<BasketItemVM>();

            if (Request.Cookies["Basket"] is not null)
            {
                //Cookiesd-de olan datani saxlamaq ucun//
                List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                foreach (var basketCookieItem in basket)
                {
                    Product product = await _db.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p=>p.Id==basketCookieItem.Id);
                    if (product is not null)
                    {
                        //baskItemVM istifadeciye gostermek ucun//
                        BasketItemVM basketItemVM = new BasketItemVM
                        {
                            Id=product.Id,
                            Name=product.Name,
                            Image=product.ProductImages.FirstOrDefault().Url,
                            Price=product.Price,
                            Count=basketCookieItem.Count,
                            SubTotal=product.Price* basketCookieItem.Count
                        };
                        basketVM.Add(basketItemVM);
                    }
                }
            }
            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if(id<=0) return BadRequest();

            Product product = await _db.Products.FirstOrDefaultAsync(p=>p.Id==id);

            if(product is null) return NotFound();
            List<BasketCookieItemVM> basket;

            if (Request.Cookies["Basket"] is not null)
            {
                basket =JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                BasketCookieItemVM item = basket.FirstOrDefault(b=>b.Id==id);
                if(Request.Cookies["Basket"] is null)
                {
                    
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basket.Add(basketCookieItemVM);
                }
                else
                {
                    item.Count++;
                }
            }
            else
            {
                basket=new List<BasketCookieItemVM>();
                BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                {
                    Id = id,
                    Count = 1
                };
                basket.Add(basketCookieItemVM);
            }

            

            string json= JsonConvert.SerializeObject(basket);

            Response.Cookies.Append("Basket", json);

            return RedirectToAction(nameof(Index),"Home");
        }
        public async Task<IActionResult> GetBasket()
        {

            return Content(Request.Cookies["Basket"]);
        }

        public async Task<IActionResult> RemoveBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _db.Products.FirstOrDefaultAsync(p=>p.Id==id);

            if (product is null) return NotFound();
            List<BasketCookieItemVM> basket;

            if (Request.Cookies["Basket"] is not null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                BasketCookieItemVM item = basket.FirstOrDefault(b => b.Id == id);

                if(item is not  null)
                {
                    basket.Remove(item);

                    string json = JsonConvert.SerializeObject(basket);
                    Response.Cookies.Append("Basket", json);
                }

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
