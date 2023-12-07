using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.Areas.Admin.ViewModels;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;
using System.Drawing;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class TagController : Controller
    {
        private readonly AppDbContext _db;
        public TagController(AppDbContext db)
        {
            _db=db;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Tag> tags= await _db.Tags.Include(t=>t.ProductTags).ToListAsync();

            return View(tags);
        }

        //Get//
        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result =  _db.Tags.Any(t=>t.Name== tagVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir tag movcuddur");
                return View();
            }

            Tag tag = new Tag
            {
                Name = tagVM.Name
            };

            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();

            Tag tag = await _db.Tags.FirstOrDefaultAsync(t=>t.Id==id); 

            if (tag is null) return NotFound();

            UpdateTagVM tagVM = new UpdateTagVM
            {
                Name = tag.Name
            };

            return View(tagVM);         
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateTagVM tagVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            Tag existed =await _db.Tags.FirstOrDefaultAsync(t=>t.Id==id);
            if (existed is null) return NotFound();

            bool result = _db.Tags.Any(t => t.Name.ToLower().Trim() == tagVM.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda artiq tag var");
                return View(tagVM);
            }

            existed.Name=tagVM.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            if(Id<=0) return BadRequest();

            Tag existed=await _db.Tags.FirstOrDefaultAsync(t=>t.Id==Id);

            if (existed is null) return NotFound();

            _db.Tags.Remove(existed);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Detail(int id)
        {
            if(id<=0) return BadRequest();

            Tag tag = _db.Tags.FirstOrDefault(t => t.Id == id);

            if (tag is null) return NotFound();

            return View(tag);   
            
        }
    }
}
