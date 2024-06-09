using BackendMiniTask.DAL;
using BackendMiniTask.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendMiniTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _context.Slider.ToListAsync();
            var categories = await _context.Categories.Include(m => m.Products).ToListAsync();

            HomeVM datas = new HomeVM()
            {
                Sliders = sliders,
                Categories = categories
            };
            return View(datas);
        }
    }
}
