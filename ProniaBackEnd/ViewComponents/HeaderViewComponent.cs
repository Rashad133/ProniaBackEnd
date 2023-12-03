using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;

namespace ProniaBackEnd.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;
        public HeaderViewComponent(AppDbContext db,IHttpContextAccessor http)
        {
            _db = db;
            _http=http;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Dictionary<string,string> settings = await _db.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            HeaderVM headerVM = new HeaderVM
            {
                Settings = await _db.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value),
                Items=await GetBasketItem()
            };
            return View(headerVM);
           

        }

        public async Task<List<BasketItemVM>> GetBasketItem()
        {
            List<BasketItemVM> basketVM = new List<BasketItemVM>();

            if (_http.HttpContext.Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(_http.HttpContext.Request.Cookies["Basket"]);

                foreach (var basketCookieItem in basket)
                {
                    Product product = await _db.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);
                    if (product is not null)
                    {
                        BasketItemVM basketItemVM = new BasketItemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Image = product.ProductImages.FirstOrDefault().Url,
                            Price = product.Price,
                            Count = basketCookieItem.Count,
                            SubTotal = product.Price * basketCookieItem.Count
                        };
                        basketVM.Add(basketItemVM);
                    }
                }
            }
            return basketVM;
        }
    }
}
