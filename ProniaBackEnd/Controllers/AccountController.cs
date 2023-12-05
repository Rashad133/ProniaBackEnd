using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaBackEnd.Models;
using ProniaBackEnd.ViewModels;

namespace ProniaBackEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManger,SignInManager<AppUser> signInManager)
        {
            _userManager = userManger;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = new AppUser()
            {
                Name=userVM.Name,
                Surname=userVM.Surname,
                Email=userVM.Email,
                UserName=userVM.Username,

            };
            IdentityResult result=await _userManager.CreateAsync(user,userVM.Password);

            if(!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                    
                }
                return View();
            }

            await _signInManager.SignInAsync(user,isPersistent:false);
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
