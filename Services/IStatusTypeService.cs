using Buisness.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buisness.Services
{
    public interface IStatusTypeService
    {
        Task<bool> CreateStatusTypeAsync(StatusTypeRegistrationForm form);
        Task<IEnumerable<StatusType>> GetAllStatusTypesAsync();
        Task<StatusType?> GetStatusTypeByIdAsync(int id);
        Task<bool> UpdateStatusTypeAsync(StatusType model);
        Task<bool> DeleteStatusTypeAsync(int id);
    }
}
