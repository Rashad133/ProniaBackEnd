using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaBackEnd.Models;
using ProniaBackEnd.Utilities.Enums;
using ProniaBackEnd.ViewModels;

namespace ProniaBackEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<AppUser> userManger,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                Gender = userVM.Gender.ToString(),

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
            await _userManager.AddToRoleAsync(user,UserRole.Member.ToString());

            await _signInManager.SignInAsync(user,isPersistent:false);
            return RedirectToAction("Index","Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Login(LoginVM loginVM,string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user= await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if(user is null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if(user is null)
                {
                    ModelState.AddModelError(String.Empty,"Username,Email or Password is incorrect");
                    return View();
                }
                   
            }

            var result=await _signInManager.PasswordSignInAsync(user,loginVM.Password,loginVM.IsRemembered,true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Your account is currently blocked, check back later");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty,"Username,Email or Password is incorrect");
                return View();
            }
            if(returnUrl is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> CreateRoles()
        {
            foreach (string role in Enum.GetNames(typeof(UserRole)))
            {
                if(!(await _roleManager.RoleExistsAsync(role.ToString())))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString(),
                    });
                }
            }
            return RedirectToAction("Index","Home");
        }
    }
}
