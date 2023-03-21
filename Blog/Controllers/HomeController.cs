using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Diagnostics;
using X.PagedList;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _context)
        {
            _logger = logger;
            this._context = _context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var setting = _context.settings!.ToList();
            var vm = new HomeVM();
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            vm.posts = await _context.posts!.Include(x => x.ApplicationUser).OrderByDescending(x => x.CreatedDate).ToPagedListAsync(pageNumber, pageSize);
            vm.Title = setting[0].Title;
            vm.ShortDescription = setting[0].ShortDescription;
            vm.ThumbnailUrl = setting[0].ThumbnailUrl;

            
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}