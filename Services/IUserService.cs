using Buisness.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buisness.Services
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserRegistrationForm form);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    }
}
