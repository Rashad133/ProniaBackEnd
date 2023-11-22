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
    }
}
