
using EcommerceApi.Data;
using EcommerceApi.Models;
using EcommerceApi.Repositories.Contracts;

namespace EcommerceApi.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool BuyProduct(int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public bool CreateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public bool DeleteProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Product? GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public Product? GetProducts()
        {
            throw new NotImplementedException();
        }

        public ICollection<Product> GetProductsByCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public bool ProductExists(int productId)
        {
            throw new NotImplementedException();
        }

        public bool ProductExists(string productName)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public ICollection<Product> SearchProduct(string productName)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}