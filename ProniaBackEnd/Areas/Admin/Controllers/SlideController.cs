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
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class SlideController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public SlideController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _db.Slides.ToListAsync();

            return View(slides);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //if (slideVM.Photo is null)
            //{
            //    ModelState.AddModelError("Photo", "Sekil secilmelidir");
            //    return View();
            //}
            if (!slideVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "File tipi uygun deyil ");
                return View();
            }
            if (slideVM.Photo.ValidateSize(2*1024))
            {
                ModelState.AddModelError("Photo", "Sekil 2mb-dan boyuk olmamalidir");
                return View();
            }

            
            string fileName = await slideVM.Photo.CreateFile(_env.WebRootPath,"assets","images","slider");

            Slide slide = new Slide
            {
                Image = fileName,
                Title = slideVM.Title,
                SubTitle = slideVM.SubTitle,
                Description = slideVM.Description,
                Order = slideVM.Order
            };


            await _db.Slides.AddAsync(slide);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Slide existed = await _db.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed is null) return NotFound();

            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                Image = existed.Image,
                Title = existed.Title,
                SubTitle = existed.SubTitle,
                Description = existed.Description,
                Order = existed.Order
            };

            return View(slideVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM slideVM)
        {

            
            if (!ModelState.IsValid)
            {
                return View(slideVM);
            }
            Slide existed = await _db.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed is null) return NotFound();


            if (slideVM.Photo is not null)
            {

                if (!slideVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File tipi uygun deyil ");
                    return View(slideVM);
                }
                if (slideVM.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "Sekil 2mb-dan boyuk olmamalidir");
                    return View(slideVM);
                }

                string newImage = await slideVM.Photo.CreateFile(_env.WebRootPath, "assets", "images", "slider");
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
                existed.Image = newImage;

            }
            existed.Title = slideVM.Title;
            existed.Description = slideVM.Description;    
            existed.SubTitle = slideVM.SubTitle;
            existed.Order = slideVM.Order;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            

        }
        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Slide slide = _db.Slides.FirstOrDefault(s => s.Id == id);

            if (slide is null) return NotFound();

            return View(slide);
        }

        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Slide slide = await _db.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slide is null) return NotFound();

            slide.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider");

            _db.Slides.Remove(slide);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }



    }
}
