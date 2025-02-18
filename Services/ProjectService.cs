using Buisness.Factories;
using Buisness.Models;
using DataStorage.Contexts;
using DataStorage.Entities;
using DataStorage.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buisness.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _dbContext;
        private readonly IRepository<ProjectEntity> _projectRepository;
        private readonly IRepository<CustomerEntity> _customerRepository;
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly IRepository<StatusTypeEntity> _statusTypeRepository;
        private readonly IRepository<UserEntity> _userRepository;


        public ProjectService(IRepository<ProjectEntity> projectRepository, IRepository<CustomerEntity> customerRepository, IRepository<ProductEntity> productRepository, IRepository<StatusTypeEntity> statusTypeRepository, IRepository<UserEntity> userRepository, DataContext dbContext)
        {
            _projectRepository = projectRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _statusTypeRepository = statusTypeRepository;
            _userRepository = userRepository;
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<string> GenerateProjectNumberAsync()
        {
            var lastProject = await _projectRepository.GetAllAsync();
            var lastNumber = lastProject.OrderByDescending(p => p.Id).FirstOrDefault()?.ProjectNumber;

            if (lastNumber == null)
                return "P-101";

            int numericPart = int.Parse(lastNumber.Split('-')[1]) + 1;
            return $"P-{numericPart}";
        }

        public async Task<bool> CreateProjectAsync(ProjectRegistrationForm form)
        {
            Console.WriteLine("CreateProjectAsync method called");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var customer = await _customerRepository.GetAsync(c => c.Id == form.CustomerId);
                if (customer == null)
                {
                    Console.WriteLine($"Customer with ID {form.CustomerId} not found!");
                    return false;
                }

                var product = await _productRepository.GetAsync(p => p.Id == form.ProductId);
                if (product == null)
                {
                    Console.WriteLine($"Product with ID {form.ProductId} not found!");
                    return false;
                }

                var statusType = await _statusTypeRepository.GetAsync(s => s.Id == form.StatusId);
                if (statusType == null)
                {
                    Console.WriteLine($"StatusType with ID {form.StatusId} not found!");
                    return false;
                }

                var user = await _userRepository.GetAsync(u => u.Id == form.UserId);
                if (user == null)
                {
                    Console.WriteLine($"User with ID {form.UserId} not found!");
                    return false;
                }

                var projectEntity = ProjectFactory.Create(form);
                if (projectEntity == null) return false;

                projectEntity.ProjectNumber = await GenerateProjectNumberAsync();
                Console.WriteLine($"Attempting to create project with:");
                Console.WriteLine($"UserId: {form.UserId}");
                Console.WriteLine($"CustomerId: {form.CustomerId}");
                Console.WriteLine($"ProductId: {form.ProductId}");
                Console.WriteLine($"StatusId: {form.StatusId}");


                await _projectRepository.AddAsync(projectEntity);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"Project Created! ID: {projectEntity.Id}, Title: {projectEntity.Title}");
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
        public async Task<bool> UpdateProjectStatusAsync(int projectId, int statusTypeId)
        {
            var projectEntity = await _projectRepository.GetAsync(p => p.Id == projectId);
            if (projectEntity == null) return false;

            projectEntity.StatusId = statusTypeId;
            await _projectRepository.UpdateAsync(projectEntity);
            return true;
        }
        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            var projects = await _dbContext.Projects
                .Include(p => p.Customer)
                .Include(p => p.Product)
                .Include(p => p.StatusType)
                .Include(p => p.User)
                .ToListAsync();

            Console.WriteLine($"Projects found in database: {projects.Count}");

            foreach (var p in projects)
            {
                Console.WriteLine($"ID: {p.Id}, Title: {p.Title}, CustomerID: {p.CustomerId}, StatusID: {p.StatusId}");
            }

            return projects.Select(ProjectFactory.Create).Where(p => p != null)!;
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            var project = await _dbContext.Projects
                .Include(p => p.Customer)
                .Include(p => p.Product)
                .Include(p => p.StatusType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            return project != null ? ProjectFactory.Create(project) : null;
        }

        public async Task<bool> UpdateProjectAsync(Project updatedProject)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                Console.WriteLine($"Attempting to update project with ID: {updatedProject.Id}");

                var project = await _projectRepository.GetAsync(p => p.Id == updatedProject.Id);
                if (project == null)
                {
                    Console.WriteLine($"Project with ID {updatedProject.Id} not found!");
                    return false;
                }

                project.Title = updatedProject.Title;
                project.Description = updatedProject.Description;
                project.StartDate = updatedProject.StartDate;
                project.EndDate = updatedProject.EndDate;
                project.StatusId = updatedProject.StatusId;

                if (updatedProject.UserId > 0)
                {
                    project.UserId = updatedProject.UserId;
                }
                else
                {
                    updatedProject.UserId = project.UserId;
                }
                await _projectRepository.UpdateAsync(project);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"Project {updatedProject.Id} updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error updating project: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            Console.WriteLine($"🔄 Attempting to delete project ID: {projectId}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (project == null)
                {
                    Console.WriteLine($"❌ Project with ID {projectId} not found.");
                    return false;
                }

                _dbContext.Projects.Remove(project);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"✅ Project {projectId} deleted successfully!");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"❌ Transaction failed: {ex.Message}");
                return false;
            }
        }
    }
}
