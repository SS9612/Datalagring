using Buisness.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buisness.Services
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(ProductRegistrationForm form);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}
