using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notyfService;

        public UserController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, INotyfService _notyfService)
        {
            this._userManager = _userManager;
            this._signInManager = _signInManager;
            this._notyfService = _notyfService;
        }

        
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View(new LoginVM());
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                
                return View(vm);
               
            }
            var existinguser = _userManager.Users.FirstOrDefault(x => x.UserName == vm.UserName);
            if(existinguser == null)
            {
                _notyfService.Error("Username does not exist");
                return View(vm);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(existinguser, vm.Password);
            if (!verifyPassword)
            {
                _notyfService.Error("Password does not match");
                return View(vm);
            }
            await _signInManager.PasswordSignInAsync(vm.UserName, vm.Password, vm.RememberMe, true);
            _notyfService.Success("Login Success");
            return RedirectToAction("Index", "User", new { area = "Admin" });

        }

        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notyfService.Success("You are logged out");
            return RedirectToAction("Index", "Home", new {area=""});
        }
    }

}
