using Buisness.Factories;
using Buisness.Models;
using DataStorage.Contexts;
using DataStorage.Entities;
using DataStorage.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buisness.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly DataContext _dbContext;

        public ProductService(DataContext dbContext, IRepository<ProductEntity> productRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _productRepository = productRepository;
        }

        public async Task<bool> CreateProductAsync(ProductRegistrationForm form)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var productEntity = ProductFactory.Create(form);
                if (productEntity == null) return false;

                await _productRepository.AddAsync(productEntity);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(ProductFactory.Create).Where(st => st != null)!;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetAsync(x => x.Id == id);
            return product != null ? ProductFactory.Create(product) : null;
        }

        public async Task<bool> UpdateProductAsync(Product updatedProduct)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var product = await _productRepository.GetAsync(p => p.Id == updatedProduct.Id);
                if (product == null) return false;

                product.ProductName = updatedProduct.ProductName;
                product.Price = updatedProduct.Price;

                await _productRepository.UpdateAsync(product);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var product = await _productRepository.GetAsync(x => x.Id == id);
                if (product == null) return false;

                await _productRepository.RemoveAsync(product);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                return false;
            }
        }
    }
}
