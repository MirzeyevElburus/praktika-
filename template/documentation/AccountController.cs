using Landab202.Areas.Admin.ViewModels;
using Landab202.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Landab202.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _man;
        private readonly SignInManager<AppUser> _sign;
        private readonly RoleManager<IdentityRole> _role;

        public AccountController(UserManager<AppUser> man,SignInManager<AppUser> sign,RoleManager<IdentityRole> role)
        {
            _man = man;
            _sign = sign;
            _role = role;
        }
        public IActionResult Registr()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Registr(RegistrVM vm)
        {
            if(!ModelState.IsValid) return View(vm);
            AppUser user = new AppUser
            {
                Email = vm.Email,
                Name = vm.Name,
                Surname = vm.Surname,
                UserName = vm.UserName,
            };
            var result=await _man.CreateAsync(user,vm.Password);
            if(!result.Succeeded)
            {
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                }
                return View(vm);
            }
            await _sign.SignInAsync(user, false);
            return RedirectToAction("Index", "Home", new {area=""});
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            AppUser user = await _man.FindByNameAsync(vm.UserNameOrEmail);
            if(user == null)
            {
                user = await _man.FindByEmailAsync(vm.UserNameOrEmail);
                if(user == null)
                {
                    ModelState.AddModelError(String.Empty, "Username ,Email or Password incorrect");
                }
                return View(vm);
            }
            var result = await _sign.PasswordSignInAsync(user, vm.Password, vm.IsRemembered, false);
            if(result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Account incorrect try again ");
                return View(vm);    
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username ,Email or Password incorrect");
                return View(vm);
            }
            await _sign.SignInAsync(user, false);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public async Task<IActionResult> Logout()
        {
            await _sign.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public async Task<IActionResult> CreateRole()
        {
            await _role.CreateAsync(new IdentityRole
            {
                Name="Admin"
            });
             return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
