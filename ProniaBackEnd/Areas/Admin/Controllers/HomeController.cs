using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProniaBackEnd.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index()
        {
           
            return View();
        }
    }
}
