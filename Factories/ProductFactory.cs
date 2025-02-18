using Buisness.Models;
using DataStorage.Entities;

namespace Buisness.Factories
{
    public static class ProductFactory
    {
        public static ProductEntity? Create(ProductRegistrationForm? form) => form is null ? null : new ProductEntity()
        {
            ProductName = form.ProductName,
            Price = form.Price
        };

        public static Product? Create(ProductEntity? entity) => entity is null ? null : new Product()
        {
            Id = entity.Id,
            ProductName = entity.ProductName,
            Price = entity.Price
        };
    }
}
