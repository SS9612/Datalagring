using Buisness.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buisness.Services
{
    public interface IProjectService
    {
        Task<bool> CreateProjectAsync(ProjectRegistrationForm form);
        Task<IEnumerable<Project>> GetProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<bool> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int id);
        Task<bool> UpdateProjectStatusAsync(int projectId, int statusTypeId);
    }
}
