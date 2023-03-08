using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.Models;
using Blog.Utilities;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.Users.ToListAsync();
            var vm = user.Select(x => new UserVM()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Email = x.Email,
            }).ToList();
            //assigning role
            foreach(var result in vm)
            {
                var singleUser = await _userManager.FindByIdAsync(result.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                result.Role = role.FirstOrDefault();
            }
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (checkUserByEmail != null)
            {
                _notyfService.Error("Email already exists");
                return View(vm);
            }
            var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName);
            if (checkUserByUsername != null)
            {
                _notyfService.Error("Username already exists");
                return View(vm);
            }

            var applicationUser = new ApplicationUser()
            {
                Email = vm.Email,
                UserName = vm.UserName,
                FirstName = vm.FirstName,
                LastName = vm.LastName
            };

            var result = await _userManager.CreateAsync(applicationUser, vm.Password);
            if (result.Succeeded)
            {
                if (vm.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAdmin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAuther);
                }
                _notyfService.Success("User registered successfully");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string Id)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);
            if(existingUser == null)
            {
                _notyfService.Error("User not Found");
                return View();
            }
            var vm = new ResetPasswordVM()
            {
                Id = existingUser.Id,
                Username = existingUser.UserName,
            };
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var existingUser = await _userManager.FindByIdAsync(vm.Id);
            if(existingUser == null)
            {
                _notyfService.Error("user dosenot exist ");
                return View();

            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
           await _userManager.ResetPasswordAsync(existingUser,token,vm.Password);
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post", new { area = "Admin" });
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
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notyfService.Success("You are logged out");
            return RedirectToAction("Index", "Home", new {area=""});
        }

        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}
