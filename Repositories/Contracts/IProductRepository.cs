
using EcommerceApi.Models;

namespace EcommerceApi.Repositories.Contracts
{
    public interface IProductRepository
    {
        Product? GetProducts();
        ICollection<Product> GetProductsByCategory(int categoryId);
        ICollection<Product> SearchProduct(string productName);
        Product? GetProduct(int id);
        bool BuyProduct(int productId, int quantity);
        bool ProductExists(int productId);
        bool ProductExists(string productName);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Product product);
        bool Save();
    }
}