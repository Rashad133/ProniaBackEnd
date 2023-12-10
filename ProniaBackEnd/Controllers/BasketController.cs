using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Interfaces;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Threading;

namespace ProniaBackEnd.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        
        public BasketController(AppDbContext db,UserManager<AppUser> userManager,IEmailService emailService)
        {
            _db = db;
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM= new List<BasketItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi=>bi.OrderId==null))
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi=>pi.IsPrimary==true))
                    .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (BasketItem item in user.BasketItems)
                {
                    basketVM.Add(new BasketItemVM
                    {
                        Name=item.Product.Name,
                        Price=item.Product.Price,
                        Count=item.Count,
                        SubTotal=item.Count*item.Product.Price,
                        Image= item.Product.ProductImages.FirstOrDefault()?.Url
                    });
                }
            }

            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    //Cookies-de olan datani saxlamaq ucun//
                    List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    foreach (var basketCookieItem in basket)
                    {
                        Product product = await _db.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);
                        if (product is not null)
                        {
                            //baskItemVM istifadeciye gostermek ucun//
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
            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if(id<=0) return BadRequest();

            Product product = await _db.Products.FirstOrDefaultAsync(p=>p.Id==id);

            if(product is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {//DB//
                AppUser user = await _userManager.Users.Include(u => u.BasketItems.Where(bi=>bi.OrderId==null)).FirstOrDefaultAsync(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (user is null) return NotFound();

                BasketItem item=user.BasketItems.FirstOrDefault(b=>b.ProductId==id);
                if(item is null)
                {
                    item = new BasketItem
                    {
                        AppUserId=user.Id,
                        ProductId=product.Id,
                        Price=product.Price,
                        Count=1,
                        OrderId=null
                    };
                    //await _db.BasketItems.AddAsync(item);
                    user.BasketItems.Add(item);
                }
                else
                {
                    item.Count++;
                    
                }

                await _db.SaveChangesAsync();
            }
            else
            {//Cookies//
                List<BasketCookieItemVM> basket;

                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    BasketCookieItemVM item = basket.FirstOrDefault(b => b.Id == id);
                    if (Request.Cookies["Basket"] is not null)
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
                    basket = new List<BasketCookieItemVM>();
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basket.Add(basketCookieItemVM);
                }



                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("Basket", json);
            }




            return RedirectToAction(nameof(Index), "Home");
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

        public IActionResult CountMinus(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

            BasketCookieItemVM item = basket.FirstOrDefault(c => c.Id == id);

            if (item == null)
            {
                return BadRequest();
            }

            item.Count--;

            if (item.Count <= 0)
            {
                basket.Remove(item);
            }

            string json = JsonConvert.SerializeObject(basket);

            Response.Cookies.Append("Basket", json, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            });

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CountPlus(int id)
        {
            List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

            BasketCookieItemVM item = basket.FirstOrDefault(c => c.Id == id);

            if (item == null)
                return BadRequest();

            item.Count++;

            if (item.Count <= 0)
            {
                basket.Remove(item);
            }

            string json = JsonConvert.SerializeObject(basket);

            Response.Cookies.Append("Basket", json, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            });

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            AppUser user = await _userManager.Users
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));

            OrderVM orderVM = new OrderVM
            {
                BasketItems = user.BasketItems
            };

            return View(orderVM);
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            AppUser user = await _userManager.Users
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid)
            {
                orderVM.BasketItems = user.BasketItems;
                return View(orderVM);
            }

            decimal total = 0;
            foreach (BasketItem item in user.BasketItems)
            {
                item.Price = item.Product.Price;
                total += item.Count * item.Price;
            }

            Order order = new Order
            {
                Status = null,
                Address=orderVM.Address,
                PurchaseAt=DateTime.Now,
                AppUserId=user.Id,
                BasketItems=user.BasketItems,
                TotalPrice=total
            };

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            string body = @"<table class=""table"">
                            <thead>
                               <tr class=""success"">
                                    <th>Name</th>
                                    <th>Price</th>
                                    <th>Count</th>
                                </tr>
                            </thead>
                            <tbody>";
            foreach (var item in order.BasketItems)
            {
                body += @$"<tr class=""info"">
                         <td>{item.Product.Name}</td>
                         <td>{item.Price}</td>
                         <td>{item.Count}</td>
                         </tr>";
            }
            body += @"</tbody>
                 </table>";
            await _emailService.SendMailAsync(user.Email,"Your Order",body,true);

            return RedirectToAction("Index","Home");
        }

    }
}
