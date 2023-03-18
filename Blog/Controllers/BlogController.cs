using Blog.Data;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Post(string slug)
        {
            if (slug == null)
            {
                return View("Error");
            }
            var post = await _context.posts!.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Slug == slug);
            if (post == null)
            {
                return View();
            }
            var vm = new BlogPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.ApplicationUser!.FirstName + " " + post.ApplicationUser.LastName,
                CreatedDate = post.CreatedDate,
                ThumbnailUrl = post.ThumbnailUrl,
                ShortDescription = post.ShortDescription,
                Description = post.Description,

            };
            return View(vm);
        }

       
    }
}
