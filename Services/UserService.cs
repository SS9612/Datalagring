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
    public class UserService : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly DataContext _dbContext;

        public UserService(DataContext dbContext, IRepository<UserEntity> userRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userRepository = userRepository;
        }

        public async Task<bool> CreateUserAsync(UserRegistrationForm form)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var userEntity = UserFactory.Create(form);
                if (userEntity == null) return false;

                await _userRepository.AddAsync(userEntity);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(UserFactory.Create!).Where(user => user is not null)!;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetAsync(x => x.Id == id);
            return user is not null ? UserFactory.Create(user) : null;
        }

        public async Task<bool> UpdateUserAsync(User updatedUser)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetAsync(u => u.Id == updatedUser.Id);
                if (user == null) return false;

                user.FirstName = updatedUser.Firstname;
                user.LastName = updatedUser.Lastname;
                user.Email = updatedUser.Email;

                await _userRepository.UpdateAsync(user);
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
        public async Task<bool> DeleteUserAsync(int id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetAsync(x => x.Id == id);
                if (user == null) return false;

                await _userRepository.RemoveAsync(user);
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
