
using EcommerceApi.Data;
using EcommerceApi.Models;
using EcommerceApi.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool BuyProduct(string name, int quantity)
        {
            if(string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return false;
            }

            var product = _db.Products.FirstOrDefault( (p)=> p.Name.ToLower().Trim() == name.ToLower().Trim());

            if(product is null || product.Stock < quantity)
            {
                return false;
            }

            product.Stock -= quantity;
            _db.Products.Update(product);

            return Save();
        }

        public bool CreateProduct(Product product)
        {
            if(product is null)
            {
                return false;
            }

            product.CreationDate = DateTime.Now;
            product.UpdateDate = DateTime.Now;

            _db.Products.Add(product);

            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            if(product is null)
            {
                return false;
            }

            _db.Products.Remove(product);

            return Save();
        }

        public Product? GetProduct(int id)
        {
            if(id <= 0)
            {
                return null;
            }

            return _db.Products.Include(p => p.Category ).FirstOrDefault(p => p.Id == id);
        }

        public ICollection<Product> GetProducts()
        {
            return _db.Products.Include(p => p.Category )
                                .OrderBy((p)=> p.Name)
                                .ToList();
        }

        public ICollection<Product> GetProductsByCategory(int categoryId)
        {
            if(categoryId <= 0)
            {
                return new List<Product>();
            }

            return _db.Products
                        .Include(p=> p.Category)
                        .Where(p => p.CategoryId == categoryId)
                        .OrderBy((p)=> p.Name)
                        .ToList();
        }

        public bool ProductExists(int productId)
        {
            if(productId <= 0)
            {
                return false;
            }

            return _db.Products.Any(p => p.Id == productId);
        }

        public bool ProductExists(string productName)
        {
            if(string.IsNullOrWhiteSpace(productName))
            {
                return false;
            }

            return _db.Products.Any(p => p.Name.ToLower().Trim() == productName.ToLower().Trim());
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        public ICollection<Product> SearchProduct(string productName)
        {
            if(string.IsNullOrWhiteSpace(productName))
            {
                return new List<Product>();
            }

            return _db.Products.Where( (p) => p.Name.ToLower().Trim().Contains(productName.ToLower().Trim()) )
                                .OrderBy((p)=> p.Name)
                                .ToList();
        }

        public bool UpdateProduct(Product product)
        {
            if(product is null)
            {
                return false;
            }

            product.UpdateDate = DateTime.Now;

            _db.Products.Update(product);

            return Save();
        }
    }
}