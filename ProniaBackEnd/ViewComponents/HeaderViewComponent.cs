using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;
using System.Security.Claims;

namespace ProniaBackEnd.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;
        public HeaderViewComponent(AppDbContext db,IHttpContextAccessor http,UserManager<AppUser> userManager)
        {
            _db = db;
            _http=http;
            _userManager=userManager;
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

            if(_http.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (BasketItem item in user.BasketItems)
                {
                    basketVM.Add(new BasketItemVM
                    {
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        Count = item.Count,
                        SubTotal = item.Count * item.Product.Price,
                        Image = item.Product.ProductImages.FirstOrDefault()?.Url
                    });
                }
            }
            else
            {
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
            }
            
            return basketVM;
        }
    }
}
