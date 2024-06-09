using BackendMiniTask.DAL;
using BackendMiniTask.Helpers.Extentions;
using BackendMiniTask.Models;
using BackendMiniTask.Services.Interfaces;
using BackendMiniTask.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendMiniTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IProductService productService, ICategoryService categoryService, IWebHostEnvironment env)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var paginateDatas = await _productService.GetAllPaginateAsync(page);
            var mappedDatas = _productService.GetMappedDatas(paginateDatas);

            ViewBag.pageCount = await GetPageCountAsync(4);
            ViewBag.currentPage = page;

            return View(mappedDatas);
        }

        private async Task<int> GetPageCountAsync(int take)
        {
            int count = await _productService.GetCountAsync();
            return (int)Math.Ceiling((decimal)count / take);
        }


        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            Product product = await _productService.GetByIdAsync((int)id);

            if (product is null) return NotFound();


            List<ProductImageVM> productImages = new();

            foreach (var item in product.ProductImage)
            {
                productImages.Add(new ProductImageVM
                {
                    Image = item.Name,
                    IsMain = item.IsMain
                });
            }

            ProductDetailVM model = new()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category.Name,
                Images = productImages
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.categories = await _categoryService.GetAllBySelectedAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM request)
        {
            ViewBag.categories = await _categoryService.GetAllBySelectedAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            foreach (var item in request.Images)
            {
                if (!item.CheckFileSize(500))
                {
                    ModelState.AddModelError("Images", "Image size must be max 500 kb");
                    return View();
                }

                if (!item.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Images", "File must be only image");
                    return View();
                }
            }

            List<ProductImage> images = new();


            foreach (var item in request.Images)
            {
                string fileName = Guid.NewGuid().ToString() + " " + item.FileName;

                string path = Path.Combine(_env.WebRootPath, "assets", "images", fileName);

                await item.SaveFileToLocalAsync(path);

                images.Add(new ProductImage
                {
                    Name = fileName
                });

            }

            images.FirstOrDefault().IsMain = true;


            Product product = new()
            {
                Name = request.Name,
                Description = request.Description,
                Price = decimal.Parse(request.Price.Replace(".", ",")),
                CategoryId = request.CategoryId,
                ProductImage = images
            };

            await _productService.CreateAsync(product);

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Product product = await _productService.GetByIdAsyncNoImage((int)id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.categories = await _categoryService.GetAllBySelectedAsync();

            return View(new ProductUpdateVM { Name = product.Name, Category = product.Category.Name, Description = product.Description, Price = product.Price });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(int? id, ProductUpdateVM productUpdateVM)
        {
            if (ModelState.IsValid)
            {
                ViewBag.categories = await _categoryService.GetAllBySelectedAsync();
                return View();
            }

            Product existProduct = await _productService.GetByIdAsyncNoImage((int)id);

            if (existProduct == null) return NotFound();

            existProduct.Name = productUpdateVM.Name;
            existProduct.Description = productUpdateVM.Description;
            existProduct.Price = productUpdateVM.Price;
            existProduct.CategoryId = productUpdateVM.CategoryId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Delete(int? id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.Include(m => m.Category)
                                                      .Include(m => m.ProductImage)
                                                      .Where(m => !m.SoftDelete)
                                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (product is null) return NotFound();
            foreach (var item in product.ProductImage)
            {
                item.Name.DeleteFile(_env.WebRootPath, "assets", "images");
            }
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}