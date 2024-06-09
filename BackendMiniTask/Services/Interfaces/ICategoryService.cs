
using BackendMiniTask.Models;
using BackendMiniTask.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BackendMiniTask.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetCategoriesAsync();
        Task<List<CategoryVM>> GetAllOrderByDescendingAsync();
        Task<bool> ExistAsync(string name);
        Task CreateAsync(CategoryCreateVM category);
        Task<Category> GetWithProductAsync(int id);
        Task DeleteAsync(Category category);
        Task<Category> GetByIdAsync(int id);
        Task EditAsync(Category category, CategoryUpdateVM categoryEdit);
        Task<SelectList> GetAllBySelectedAsync();
    }
}
