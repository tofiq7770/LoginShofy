using BackendMiniTask.DAL;
using BackendMiniTask.Helpers.Extentions;
using BackendMiniTask.Models;
using BackendMiniTask.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendMiniTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            var datas = await _context.Categories.ToListAsync();
            return View(datas);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM create)
        {
            if (!ModelState.IsValid) return View();

            if (!create.Image.CheckFileType("image/"))
            {
                ModelState.AddModelError("Images", "File must be Image Format");
                return View();
            }
            if (!create.Image.CheckFileSize(300))
            {

                ModelState.AddModelError("Images", "Max File Capacity mut be 300KB");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + "-" + create.Image.FileName;
            string path = Path.Combine(_env.WebRootPath, "assets", "images", fileName);
            await create.Image.SaveFileToLocalAsync(path);
            Category category = new()
            {
                Name = create.Name,
                Image = fileName
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();
            Category category = await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null) return NotFound();
            CategoryDetailVM model = new()
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image

            };
            return View(model);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);

            if (category is null) return NotFound();

            category.Name.DeleteFile(_env.WebRootPath, "assets", "images");

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id <= 0) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);

            if (category is null) return NotFound();

            return View(new CategoryUpdateVM { Image = category.Image, Name = category.Name });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, CategoryUpdateVM request)
        {
            if (id <= 0) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);

            if (category is null) return NotFound();

            if (request.Photo is not null)
            {
                if (!request.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "File must be Image Format");
                    return View();
                }
                if (!request.Photo.CheckFileSize(300))
                {

                    ModelState.AddModelError("Image", "Max File Capacity mut be 300KB");
                    return View();
                }

                category.Image.DeleteFile(_env.WebRootPath, "assets", "images");

                string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;

                string newPath = Path.Combine(_env.WebRootPath, "assets", "images", fileName);
                await request.Photo.SaveFileToLocalAsync(newPath);

                category.Image = fileName;
            }


            category.Name = request.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
