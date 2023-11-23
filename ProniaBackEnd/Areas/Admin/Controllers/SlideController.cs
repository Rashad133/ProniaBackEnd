using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _db;
        public SlideController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides=await _db.Slides.ToListAsync();

            return View(slides);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (slide.Photo is null)
            {
                ModelState.AddModelError("Photo", "Sekil secilmelidir");
                return View();
            }
            if (!slide.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "File tipi uygun deyil ");
                return View();
            }
            if (slide.Photo.Length >2*1024*1024)
            {
                ModelState.AddModelError("Photo", "Sekil 2mb-dan boyuk olmamalidir");
                return View();
            }


            FileStream file = new FileStream(@"C:\Users\ACER\Desktop\ProniaProject\ProniaBackEnd\ProniaBackEnd\wwwroot\assets\images\slider\"+slide.Photo.FileName,FileMode.Create);

            await slide.Photo.CopyToAsync(file);
            file.Close();
            slide.Image = slide.Photo.FileName;


            await _db.Slides.AddAsync(slide);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Slide slide = _db.Slides.FirstOrDefault(s=>s.Id == id);

            if(slide is null) return NotFound();

            return View(slide);
        }
    }
}
