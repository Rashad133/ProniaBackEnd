using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]    
    public class TagController : Controller
    {
        private readonly AppDbContext _db;
        public TagController(AppDbContext db)
        {
            _db=db;
        }
        public async Task<IActionResult> Index()
        {
            List<Tag> tags= await _db.Tags.Include(t=>t.ProductTags).ToListAsync();

            return View(tags);
        }

        //Get//
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result =  _db.Tags.Any(t=>t.Name.ToLower().Trim()== tag.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele bir tag movcuddur");
                return View();
            }

            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

            
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();

            Tag tag = await _db.Tags.FirstOrDefaultAsync(t=>t.Id==id); 

            if (tag is null) return NotFound();

            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,Tag tag)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            Tag existed =await _db.Tags.FirstOrDefaultAsync(t=>t.Id==id);
            if (existed is null) return NotFound();

            bool result = _db.Tags.Any(t => t.Name == tag.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda artiq tag var");
                return View();
            }

            existed.Name=tag.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            if(Id<=0) return BadRequest();

            Tag existed=await _db.Tags.FirstOrDefaultAsync(t=>t.Id==Id);

            if (existed is null) return NotFound();

            _db.Tags.Remove(existed);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Detail(int id)
        {
            if(id<=0) return BadRequest();

            Tag tag = _db.Tags.FirstOrDefault(t => t.Id == id);

            if (tag is null) return NotFound();

            return View(tag);   
            
        }
    }
}
