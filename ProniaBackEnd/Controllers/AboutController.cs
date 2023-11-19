using Microsoft.AspNetCore.Mvc;

namespace ProniaBackEnd.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
