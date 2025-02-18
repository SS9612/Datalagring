
using Buisness.Models;
using DataStorage.Entities;
namespace Buisness.Factories
{
    public static class CustomerFactory
    {
        public static CustomerEntity? Create(CustomerRegistrationForm? form) => form is null ? null : new CustomerEntity()
        {
            CustomerName = form.CustomerName
        };

        public static Customer? Create(CustomerEntity? entity) => entity is null ? null : new Customer()
        {
            Id = entity.Id,
            CustomerName = entity.CustomerName
        };
    }
}
