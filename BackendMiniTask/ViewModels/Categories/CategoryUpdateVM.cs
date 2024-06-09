namespace BackendMiniTask.ViewModels.Categories
{
    public class CategoryUpdateVM
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
