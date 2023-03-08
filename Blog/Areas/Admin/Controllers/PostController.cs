using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.Data;
using Blog.Migrations;
using Blog.Models;
using Blog.Utilities;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(UserManager<ApplicationUser> _userManager, ApplicationDbContext _context, INotyfService _notyfService, IWebHostEnvironment _webHostEnvironment)
        {
            this._userManager = _userManager;
            this._context = _context;
            this._notyfService = _notyfService;
            this._webHostEnvironment = _webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listOfPosts = new List<Post>();

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin)
            {
                listOfPosts = await _context.posts!.Include(x => x.ApplicationUser).ToListAsync();
            }
            else
            {
                listOfPosts = await _context.posts!.Include(x => x.ApplicationUser).Where(x => x.ApplicationUser!.Id == loggedInUser!.Id).ToListAsync();
            }

            var listOfPostsVM = listOfPosts.Select(x => new PostVM()
            {
                Id = x.Id,
                Title = x.Title,
                CreatedDate = x.CreatedDate,
                ThumbnailUrl = x.ThumbnailUrl,
                AuthorName = x.ApplicationUser!.FirstName + " " + x.ApplicationUser.LastName
            }).ToList();
            return View(listOfPostsVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            var post = new Post();

            post.Title = vm.Title;
            post.Description = vm.Description;
            post.ShortDescription = vm.ShortDescription;
            post.ApplicationUserId = loggedInUser!.Id;
            if(post.Title!= null)
            {
                string slug = vm.Title!.Trim();
                slug = slug.Replace(" ", "_");
                post.Slug = slug + "_" + Guid.NewGuid();
            }
            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadFile(vm.Thumbnail);
            };

            await _context.posts.AddAsync(post);
            await _context.SaveChangesAsync();
            _notyfService.Success("Posted successfuly");

            return RedirectToAction("Index");
        }
        private string UploadFile(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnail");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath= Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.posts!.FirstOrDefaultAsync(x => x.Id == id);

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin || loggedInUser?.Id == post?.ApplicationUserId)
            {
                _context.posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _notyfService.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.posts!.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                _notyfService.Error("Post not found");
                return View();
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] != WebsiteRoles.WebsiteAdmin && loggedInUser!.Id != post.ApplicationUserId)
            {
                _notyfService.Error("You are not authorized");
                return RedirectToAction("Index");
            }

            var vm = new CreatePostVM()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var post = await _context.posts!.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (post == null)
            {
                _notyfService.Error("Post not found");
                return View();
            }

            post.Title = vm.Title;
            post.ShortDescription = vm.ShortDescription;
            post.Description = vm.Description;

            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadFile(vm.Thumbnail);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Post updated succesfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
    }
}
