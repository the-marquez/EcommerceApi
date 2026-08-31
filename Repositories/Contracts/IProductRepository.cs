
using EcommerceApi.Models;

namespace EcommerceApi.Repositories.Contracts
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        ICollection<Product> GetProductsByCategory(int categoryId);
        ICollection<Product> SearchProducts(string productName);
        Product? GetProduct(int id);
        bool BuyProduct(string name, int quantity);
        bool ProductExists(int productId);
        bool ProductExists(string productName);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Product product);
        bool Save();
    }
}