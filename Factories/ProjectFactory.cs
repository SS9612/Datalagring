using Buisness.Models;
using DataStorage.Entities;
namespace Buisness.Factories
{
    
    public static class ProjectFactory
    {
        public static ProjectEntity Create(ProjectRegistrationForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            return new ProjectEntity()
            {
                ProjectNumber = $"P-{new Random().Next(100, 999)}",
                Title = form.Title ?? "Untitled",
                Description = form.Description ?? "No description",
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                CustomerId = form.CustomerId,
                ProductId = form.ProductId,
                StatusId = form.StatusId,
                UserId = form.UserId
            };
        }

        public static Project Create(ProjectEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new Project()
            {
                Id = entity.Id,
                ProjectNumber = entity.ProjectNumber ?? "N/A",
                Title = entity.Title ?? "Untitled",
                Description = entity.Description ?? "No description",
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CustomerId = entity.CustomerId,
                ProductId = entity.ProductId,
                StatusId = entity.StatusId,
                UserId = entity.UserId
            };
        }
    }
}