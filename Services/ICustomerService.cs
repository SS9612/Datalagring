using Buisness.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buisness.Services
{
    public interface ICustomerService
    {
        Task<bool> CreateCustomerAsync(CustomerRegistrationForm form);
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByCustomerNameAsync(string customerName);
        Task<bool> UpdateCustomerAsync(Customer updatedCustomer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
