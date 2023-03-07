using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Utilities
{
    public class Dbinitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        //user manger helps to create user
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public Dbinitializer(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            this._context = _context;
            this._userManager = _userManager;
            this._roleManager = _roleManager;
            
        }
        
        public void Initialize()
        {
            if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
            {
                //_roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
                //_roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAuther)).GetAwaiter().GetResult();
                //_userManager.CreateAsync(new ApplicationUser()
                //{
                //    UserName = "Admin@gmail.com",
                //    Email = "Admin@gmail.com",
                //    FirstName = "Super",
                //    LastName = "Admin",

                //}, "Admin123").Wait();
                //var appUser = _context.applicationUsers.FirstOrDefault(x => x.Email == "Admin@gmail.com");
                //if(appUser != null) {
                //    _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
                //}
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAuther)).GetAwaiter().GetResult();
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin"
                }, "Admin@0011").Wait();

                var appUser = _context.applicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
                if (appUser != null)
                {
                    _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
                }


                var listOfPages = new List<Page>()
                {
                    new Page()
                {
                    Title = "About Us",
                    Slug = "about"
                },

                new Page()
                {
                    Title = "Contact Us",
                    Slug = "Contact"
                },
                new Page()
                {
                    Title = "PrivayPage Us",
                    Slug = "PrivayPage"
                }
            };
                _context.pages.AddRange(listOfPages);
                _context.SaveChanges();

            }
        }
    }
}
