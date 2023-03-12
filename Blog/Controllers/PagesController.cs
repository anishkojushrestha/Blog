using Blog.Data;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class PagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult About()
        {
            var result = _context.pages.FirstOrDefault(x => x.Slug == "about");
            var vm = new PageVM()
            {
                Id = result.Id,
                Title = result.Title,
                ShortDescription = result.ShortDescription,
                Description = result.Description,
                ThumbnailUrl = result.ThumbnailUrl,
            };
            return View(vm);
        }
        public IActionResult Contact()
        {
            var result = _context.pages.FirstOrDefault(x => x.Slug == "contact");
            var vm = new PageVM()
            {
                Id = result.Id,
                Title = result.Title,
                ShortDescription = result.ShortDescription,
                Description = result.Description,
                ThumbnailUrl = result.ThumbnailUrl,
            };
            return View(vm);
        }
        public IActionResult PrivacyPolicy()
        {
            var result = _context.pages.FirstOrDefault(x => x.Slug == "PrivayPage");
            var vm = new PageVM()
            {
                Id = result.Id,
                Title = result.Title,
                ShortDescription = result.ShortDescription,
                Description = result.Description,
                ThumbnailUrl = result.ThumbnailUrl,
            };
            return View(vm);
        }
    }
}
