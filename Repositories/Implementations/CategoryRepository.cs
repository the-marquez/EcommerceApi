
using EcommerceApi.Data;
using EcommerceApi.Models;
using EcommerceApi.Repositories.Contracts;

namespace EcommerceApi.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CategoryExists(int id)
        {
            return _db.Categories.Any((c)=> c.Id == id);
        }

        public bool CategoryExists(string name)
        {
            return _db.Categories.Any((c)=> c.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool CreateCategory(Category category)
        {
            category.CreatedAt = DateTime.Now;
            _db.Categories.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _db.Categories.Remove(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            return _db.Categories.OrderBy((c)=> c.Name).ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return _db.Categories.FirstOrDefault((c)=> c.Id == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        public bool UpdateCategory(Category category)
        {
            category.CreatedAt = DateTime.Now;
            _db.Categories.Update(category);
            return Save();
        }
    }
}