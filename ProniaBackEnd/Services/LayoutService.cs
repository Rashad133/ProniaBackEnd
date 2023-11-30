using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;

namespace ProniaBackEnd.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _db;
        public LayoutService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Dictionary<string,string>> GetSettingsAsync()
        {
            Dictionary<string,string> settings= await _db.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;
        }
    }
}
