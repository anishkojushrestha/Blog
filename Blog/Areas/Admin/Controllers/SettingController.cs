using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        

        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notyfService = notyfService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = _context.settings!.ToList();
            if (settings.Count > 0)
            {
                var vm = new SettingVM()
                {
                    Id = settings[0].Id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    ShortDescription = settings[0].ShortDescription,
                    FacebookUrl = settings[0].FacebookUrl,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    TwitterUrl = settings[0].TwitterUrl,
                    GitUrl = settings[0].GitUrl,
                };
                return View(vm);

            }
            var setting = new Setting()
            {
                SiteName = "Demo Name",

            };
            await _context.settings!.AddAsync(setting);
            await _context.SaveChangesAsync();
            var createdSetting = await _context.settings!.ToListAsync();
            var createdVm = new SettingVM()
            {
                Id = createdSetting[0].Id,
                SiteName = createdSetting[0].SiteName,
                Title = createdSetting[0].Title,
                ShortDescription = createdSetting[0].ShortDescription,
                ThumbnailUrl = createdSetting[0].ThumbnailUrl,
                FacebookUrl = createdSetting[0].FacebookUrl,
                TwitterUrl = createdSetting[0].TwitterUrl,
                GitUrl = createdSetting[0].GitUrl,

            };
            return View(createdVm);

        }
        [HttpPost]
        public async Task<IActionResult> Index(SettingVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var setting = _context.settings.FirstOrDefault(x => x.Id == vm.Id);
            if(setting == null)
            {
                _notyfService.Error("Something went wrong");
                return View(vm);
            }
            setting.SiteName = vm.SiteName;
            setting.Title = vm.Title;
            setting.ShortDescription = vm.ShortDescription;
            if(vm.Thumbnail != null)
            {
                setting.ThumbnailUrl = UploadFile(vm.Thumbnail);
            }
            setting.FacebookUrl = vm.FacebookUrl;
            setting.TwitterUrl  = vm.TwitterUrl;
            setting.GitUrl  = vm.GitUrl;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        private string UploadFile(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnail");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
