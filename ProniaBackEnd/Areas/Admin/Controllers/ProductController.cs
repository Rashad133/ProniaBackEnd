using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.Areas.Admin.ViewModels;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using ProniaBackEnd.Utilities.Extensions;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext db,IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<Product> products=await _db.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages
                .Where(pi=>pi.IsPrimary==true))
                .ToListAsync();

            return View(products);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM 
            { 
                Categories =await _db.Categories.ToListAsync(),
                Tags = await _db.Tags.ToListAsync(),
                Colors= await _db.Colors.ToListAsync(),
                Sizes = await _db.Sizes.ToListAsync()

            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _db.Categories.ToListAsync();
                productVM.Tags = await _db.Tags.ToListAsync();
                productVM.Colors= await _db.Colors.ToListAsync();
                productVM.Sizes = await _db.Sizes.ToListAsync();
                return View(productVM);
            }
            bool result = await _db.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _db.Categories.ToListAsync();
                productVM.Tags = await _db.Tags.ToListAsync();
                productVM.Colors = await _db.Colors.ToListAsync();
                productVM.Sizes = await _db.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId","Bu Id-li category yoxdu");
                return View(productVM);
            }

            foreach (int tagId in productVM.TagIds)
            {
                bool tagResult = await _db.Tags.AnyAsync(t=>t.Id==tagId);
                if (!tagResult)
                {
                    productVM.Categories = await _db.Categories.ToListAsync();
                    productVM.Tags = await _db.Tags.ToListAsync();
                    productVM.Colors = await _db.Colors.ToListAsync();
                    ModelState.AddModelError("TagIds", "tag melumatlari duzgun deyil");
                    return View(productVM);
                }
            }

            foreach (int colorId in productVM.ColorIds)
            {
                bool colorResult = await _db.Colors.AnyAsync(s => s.Id == colorId);
                if (!colorResult)
                {
                    productVM.Categories= await _db.Categories.ToListAsync();
                    productVM.Tags= await _db.Tags.ToListAsync();
                    productVM.Colors = await _db.Colors.ToListAsync();
                    ModelState.AddModelError("ColorIds","Color duzgun deyil");

                    return View(productVM);
                }
            }

            foreach (int sizeId in productVM.SizeIds)
            {
                bool sizeResult = await _db.Sizes.AnyAsync(s=>s.Id==sizeId);
                if (!sizeResult)
                {
                    productVM.Categories=await _db.Categories.ToListAsync();
                    productVM.Tags=await _db.Tags.ToListAsync();
                    productVM.Sizes=await _db.Sizes.ToListAsync();
                    ModelState.AddModelError("SizeIds", "Size duzgun deyil");

                    return View(productVM);
                }
            }

            if (!productVM.MainPhoto.ValidateType())
            {
                productVM.Categories = await _db.Categories.ToListAsync();
                productVM.Tags = await _db.Tags.ToListAsync();
                productVM.Colors = await _db.Colors.ToListAsync();
                productVM.Sizes = await _db.Sizes.ToListAsync();
                ModelState.AddModelError("MainPhoto", "fayl tipi uygun deyil");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(5))
            {
                productVM.Categories = await _db.Categories.ToListAsync();
                productVM.Tags = await _db.Tags.ToListAsync();
                productVM.Colors = await _db.Colors.ToListAsync();
                productVM.Sizes = await _db.Sizes.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "Fayl olcusu uygun deyil");
                return View(productVM);
            }

            if (!productVM.HoverPhoto.ValidateType())
            {
                productVM.Categories = await _db.Categories.ToListAsync();
                productVM.Tags = await _db.Tags.ToListAsync();
                productVM.Colors = await _db.Colors.ToListAsync();
                productVM.Sizes = await _db.Sizes.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "fayl tipi uygun deyil");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateSize(5))
            {
                productVM.Categories = await _db.Categories.ToListAsync();
                productVM.Tags = await _db.Tags.ToListAsync();
                productVM.Colors = await _db.Colors.ToListAsync();
                productVM.Sizes = await _db.Sizes.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "fayl olcusu uygun deyil");
                return View(productVM);
            }

            ProductImage image = new ProductImage
            {
                Alternative=productVM.Name,
                IsPrimary = true,
                Url=await productVM.MainPhoto.CreateFile(_env.WebRootPath,"assets","images","website-images")
            };
            ProductImage hoverImage = new ProductImage
            {
                Alternative=productVM.Name,
                IsPrimary=false,
                Url=await productVM.HoverPhoto.CreateFile(_env.WebRootPath,"assets","images","website-images")
            };

            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId,
                Description = productVM.Description,
                ProductTags= new List<ProductTag>(),
                ProductColors= new List<ProductColor>(),
                ProductSizes= new List<ProductSize>(),
                ProductImages= new List<ProductImage>() { image, hoverImage }

            };



            foreach (int tagId in productVM.TagIds)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId= tagId,
                };
                product.ProductTags.Add(productTag);
            }

            foreach(int colorId in productVM.ColorIds)
            {
                ProductColor productColor = new ProductColor
                {
                    ColorId= colorId,
                };
                product.ProductColors.Add(productColor);
            }

            foreach (int sizeId in productVM.SizeIds)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId= sizeId,
                };
                product.ProductSizes.Add(productSize);
            }

            TempData["Message"] = "";
            foreach (IFormFile photo in productVM.Photos)
            {
                if (!photo.ValidateType("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil</p>";
                    continue;
                }
                if (!photo.ValidateSize(5))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil</p>";
                    continue;
                }
                product.ProductImages.Add(new ProductImage
                {
                    Alternative=productVM.Name,
                    IsPrimary=null,
                    Url=await photo.CreateFile(_env.WebRootPath,"assets","images","website-images")
                });
            }

            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = _db.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductColors)
                .ThenInclude(p=>p.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(p=>p.Size)
                .Include(p=>p.ProductTags)
                .ThenInclude(p=>p.Tag)
                .Include(p=>p.ProductImages)
                .FirstOrDefault(c => c.Id == id);

            if (product is null) return NotFound();

            return View(product );
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();

            Product product = await _db.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(p=> p.Id == id);

            if(product is null) return NotFound();

            foreach (ProductImage image in product.ProductImages)
            {
                image.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            }


            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            if(id<=0) return BadRequest();

            Product product = await _db.Products.Include(p=>p.ProductImages).Include(p=>p.ProductTags).Include(p=>p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefaultAsync(p=>p.Id==id);

            if(product is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                TagIds=product.ProductTags.Select(pt=>pt.TagId).ToList(), 
                ColorIds=product.ProductColors.Select(pt=>pt.ColorId).ToList(),
                SizeIds=product.ProductSizes.Select(pt=>pt.SizeId).ToList(),
                ProductImages=product.ProductImages,
                Categories= await _db.Categories.ToListAsync(),
                Tags= await _db.Tags.ToListAsync(),
                Colors= await _db.Colors.ToListAsync(),
                Sizes=await _db.Sizes.ToListAsync(),
            };

            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id,UpdateProductVM productVM)
        {
            Product existed = await _db.Products.Include(pi=>pi.ProductImages).Include(p => p.ProductTags).Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefaultAsync(p => p.Id == id);
            productVM.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                productVM.Categories= await _db.Categories.ToListAsync();
                productVM.Tags= await _db.Tags.ToListAsync();
                productVM.Colors= await _db.Colors.ToListAsync();
                productVM.Sizes=await _db.Sizes.ToListAsync();

                return View(productVM);
            }

            if (existed is null) return NotFound();

            bool result = await _db.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);
            if(!result)
            {
                productVM.Categories= await _db.Categories.ToListAsync();
                productVM.Tags = await _db.Tags.ToListAsync();
                productVM.Colors=await _db.Colors.ToListAsync();
                productVM.Sizes = await _db.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bele category yoxdur");
                return View(productVM);
            }

            if(productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType())
                {
                    productVM.Categories = await _db.Categories.ToListAsync();
                    productVM.Tags = await _db.Tags.ToListAsync();
                    productVM.Colors = await _db.Colors.ToListAsync();
                    productVM.Sizes = await _db.Sizes.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "file novu uyugn deyil");
                    return View(productVM);
                }
                if (productVM.MainPhoto.ValidateSize(5))
                { 
                    productVM.Categories = await _db.Categories.ToListAsync();
                    productVM.Tags = await _db.Tags.ToListAsync();
                    productVM.Colors = await _db.Colors.ToListAsync();
                    productVM.Sizes = await _db.Sizes.ToListAsync();
                    ModelState.AddModelError("Main", "file olcusu uygun deyil");
                    return View(productVM);
                }
            }

            if(productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.ValidateType())
                {
                    productVM.Categories = await _db.Categories.ToListAsync();
                    productVM.Tags = await _db.Tags.ToListAsync();
                    productVM.Colors = await _db.Colors.ToListAsync();
                    productVM.Sizes = await _db.Sizes.ToListAsync();
                    ModelState.AddModelError("HoverPhoto", "file tipi uyugn deyil");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.ValidateSize(5))
                {
                    productVM.Categories = await _db.Categories.ToListAsync();
                    productVM.Tags = await _db.Tags.ToListAsync();
                    productVM.Colors = await _db.Colors.ToListAsync();
                    productVM.Sizes = await _db.Sizes.ToListAsync();
                    ModelState.AddModelError("HoverPhoto", "file olcusu uygun deyil");
                    return View(productVM);
                }
            }

            

            //List<ProductTag> removeable = existed.ProductTags.Where(pt => !productVM.TagIds.Exists(tId => tId == pt.TagId)).ToList();
            //_db.ProductTags.RemoveRange(removeable);

            existed.ProductTags.RemoveAll(pt=>!productVM.TagIds.Exists(tId=>tId==pt.TagId));

            List<int> creatable = productVM.TagIds.Where(tId => !existed.ProductTags.Exists(pt => pt.TagId == tId)).ToList();

            foreach (int tagId in creatable)
            {
                bool tagResult = await _db.Tags.AnyAsync(t => t.Id == tagId);
                if(!tagResult)
                {
                    productVM.Categories = await _db.Categories.ToListAsync();
                    productVM.Tags = await _db.Tags.ToListAsync();
                    productVM.Colors = await _db.Colors.ToListAsync();
                    productVM.Sizes = await _db.Sizes.ToListAsync();
                    ModelState.AddModelError("CategoryId", "Bele tag yoxdur");
                    return View(productVM);
                }
                existed.ProductTags.Add(new ProductTag
                {
                    TagId=tagId
                });
                
            }

            foreach (ProductColor pColor in existed.ProductColors)
            {
                if (productVM.ColorIds.Exists(cId => cId == pColor.ColorId))
                {
                    _db.ProductColors.Remove(pColor);
                }
            }

            List<int> newColorIds = new List<int>();

            foreach (int colorId in productVM.ColorIds)
            {
                if (!existed.ProductColors.Any(pc => pc.ColorId == colorId))
                {
                    existed.ProductTags.Add(new ProductTag
                    {
                        TagId = colorId
                    });
                }
            }

            foreach (ProductSize pSize in existed.ProductSizes)
            {
                if (productVM.SizeIds.Exists(sId => sId == pSize.SizeId))
                {
                    _db.ProductSizes.Remove(pSize);
                }
            }
            List<int> newSizeIds = new List<int>();

            foreach (int sizeId in productVM.SizeIds)
            {
                if (!_db.ProductSizes.Any(ps => ps.SizeId == sizeId))
                {
                    existed.ProductSizes.Add(new ProductSize
                    {
                        SizeId = sizeId,
                    }); 

                }
            }

            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "websites-images");

                ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                _db.ProductImages.Remove(mainImage);

                existed.ProductImages.Add(new ProductImage
                {
                    Alternative = productVM.Name,
                    IsPrimary = true,
                    Url = fileName,
                });
            }

            if (productVM.HoverPhoto is not null)
            {
                string fileName = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "websites-images");

                ProductImage hoverImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                hoverImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                _db.ProductImages.Remove(hoverImage);

                existed.ProductImages.Add(new ProductImage
                {
                    Alternative = productVM.Name,
                    IsPrimary = false,
                    Url = fileName,
                });
            }
            if(productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage pImage in removeable)
            {
                pImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(pImage);
            }

            TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (IFormFile photo in productVM.Photos )
                {
                    if (!photo.ValidateType())
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil</p>";
                        continue;
                    }
                    if (!photo.ValidateSize(5))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil</p>";
                        continue;
                    }
                    existed.ProductImages.Add(new ProductImage
                    {
                        Alternative = productVM.Name,
                        IsPrimary = null,
                        Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-iamges")
                    });
                }
            }
            

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.CategoryId = productVM.CategoryId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
