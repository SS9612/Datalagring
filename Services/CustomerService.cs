using Buisness.Factories;
using Buisness.Models;
using Buisness.Services;
using DataStorage.Contexts;
using DataStorage.Entities;
using DataStorage.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CustomerService : ICustomerService
{
    private readonly IRepository<CustomerEntity> _customerRepository;
    private readonly DataContext _dbContext;

    public CustomerService(DataContext dbContext, IRepository<CustomerEntity> customerRepository)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _customerRepository = customerRepository;
    }

    public async Task<bool> CreateCustomerAsync(CustomerRegistrationForm form)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var customerEntity = CustomerFactory.Create(form);
            if (customerEntity == null) return false;

            await _customerRepository.AddAsync(customerEntity);
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

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        var customerEntities = await _customerRepository.GetAllAsync();
        return customerEntities.Select(CustomerFactory.Create).Where(st => st != null)!;
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        var customerEntity = await _customerRepository.GetAsync(x => x.Id == id);
        return customerEntity != null ? CustomerFactory.Create(customerEntity) : null;
    }

    public async Task<Customer?> GetCustomerByCustomerNameAsync(string customerName)
    {
        var customerEntity = await _customerRepository.GetAsync(x => x.CustomerName == customerName);
        return customerEntity != null ? CustomerFactory.Create(customerEntity) : null;
    }

    public async Task<bool> UpdateCustomerAsync(Customer updatedCustomer)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var customer = await _customerRepository.GetAsync(c => c.Id == updatedCustomer.Id);
            if (customer == null) return false;

            customer.CustomerName = updatedCustomer.CustomerName;

            await _customerRepository.UpdateAsync(customer);
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

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var customerEntity = await _customerRepository.GetAsync(x => x.Id == id);
            if (customerEntity == null) return false;

            await _customerRepository.RemoveAsync(customerEntity);
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

