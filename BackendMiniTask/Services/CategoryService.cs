

using BackendMiniTask.DAL;
using BackendMiniTask.Models;
using BackendMiniTask.Services.Interfaces;
using BackendMiniTask.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BackendMiniTask.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.Include(m => m.Products).Where(m => !m.SoftDelete && m.Products.Count != 0).ToListAsync();
        }
        public async Task CreateAsync(CategoryCreateVM category)
        {
            await _context.Categories.AddAsync(new Category { Name = category.Name });
            await _context.SaveChangesAsync();
        }

        public Task CreateAsync(CategoryVM category)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        public async Task EditAsync(Category category, CategoryUpdateVM categoryEdit)
        {
            category.Name = categoryEdit.Name;
            await _context.SaveChangesAsync();

        }
        public async Task<bool> ExistAsync(string name)
        {
            return await _context.Categories.AnyAsync(m => m.Name.Trim() == name.Trim());
        }
        public async Task<List<CategoryVM>> GetAllOrderByDescendingAsync()
        {
            List<Category> categories = await _context.Categories.OrderByDescending(m => m.Id)
                                                                 .ToListAsync();
            return categories.Select(m => new CategoryVM { Id = m.Id, Name = m.Name }).ToList();
        }
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.Where(m => m.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Category> GetWithProductAsync(int id)
        {
            return await _context.Categories.Where(m => m.Id == id)
                                            .Include(m => m.Products)
                                            .FirstOrDefaultAsync();
        }
        public async Task<SelectList> GetAllBySelectedAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return new SelectList(categories, "Id", "Name");
        }
    }
}
