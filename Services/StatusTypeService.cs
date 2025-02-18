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
    public class StatusTypeService : IStatusTypeService
    {
        private readonly IRepository<StatusTypeEntity> _repository;
        private readonly DataContext _dbContext;

        public StatusTypeService(DataContext dbContext, IRepository<StatusTypeEntity> repository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _repository = repository;
        }

        public async Task<IEnumerable<StatusType>> GetAllStatusTypesAsync()
        {
            var statusTypes = await _repository.GetAllAsync();
            return statusTypes.Select(StatusTypeFactory.Create!).Where(st => st != null)!;
        }

        public async Task<StatusType?> GetStatusTypeByIdAsync(int id)
        {
            var entity = await _repository.GetAsync(x => x.Id == id);
            return StatusTypeFactory.Create(entity);
        }

        public async Task<bool> CreateStatusTypeAsync(StatusTypeRegistrationForm form)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var entity = StatusTypeFactory.Create(form);
                if (entity == null) return false;

                await _repository.AddAsync(entity);
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

        public async Task<bool> UpdateStatusTypeAsync(StatusType model)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var entity = await _repository.GetAsync(x => x.Id == model.Id);
                if (entity == null) return false;

                entity.Statusname = model.Statusname;

                await _repository.UpdateAsync(entity);
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

        public async Task<bool> DeleteStatusTypeAsync(int id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var entity = await _repository.GetAsync(x => x.Id == id);
                if (entity == null) return false;

                await _repository.RemoveAsync(entity);
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
