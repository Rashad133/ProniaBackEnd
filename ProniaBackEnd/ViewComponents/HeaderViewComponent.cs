using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;

namespace ProniaBackEnd.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppDbContext _db;
        public HeaderViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string,string> settings = await _db.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return View(settings);
        }
    }
}
