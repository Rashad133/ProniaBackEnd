using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.Areas.Admin.ViewModels;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using System.Drawing;
using Size = ProniaBackEnd.Models.Size;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _db;
        public SizeController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        public async  Task<IActionResult> Index()
        {
            List<Size> sizes = await _db.Sizes.Include(s=>s.ProductSizes).ToListAsync();

            return View(sizes);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            bool result = _db.Sizes.Any(t => t.Name == sizeVM.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir tag movcuddur");
                return View();
            }

            Size size = new Size
            {
                Name = sizeVM.Name
            };

            await _db.Sizes.AddAsync(size);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Size size = await _db.Sizes.FirstOrDefaultAsync(s=>s.Id==id);

            if(size is null) return NotFound();

            UpdateSizeVM sizeVM = new UpdateSizeVM
            {
                Name = size.Name
            };

            return View(sizeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateSizeVM sizeVM)
        {
            if (!ModelState.IsValid) return View();

            Size existed = await _db.Sizes.Include(s=>s.ProductSizes).FirstOrDefaultAsync(s=>s.Id==id);
            if(existed is null) return NotFound();

            bool result =  _db.Sizes.Any(s=>s.Name==sizeVM.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda artiq size var");
                return View();
            }

            existed.Name = sizeVM.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();

            Size existed = await _db.Sizes.FirstOrDefaultAsync(s=>s.Id==id);
            if (existed is null) return NotFound();

            _db.Sizes.Remove(existed);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Size size = _db.Sizes.FirstOrDefault(s=>s.Id==id);

            if(size is null) return NotFound(); 

            return View(size);
        }
    }
}
