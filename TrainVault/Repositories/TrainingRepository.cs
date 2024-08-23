using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainVault.DataAccess;
using TrainVault.Interfaces;
using TrainVault.Models;

namespace TrainVault.Repositories
{
    public class TrainingRepository : ITraining
    {
        private readonly TrainVaultContext _context;

        public TrainingRepository(TrainVaultContext context)
        {
            _context = context;
        }

        public async Task<bool> CanCreateTrainingAsync(int organizationId, DateOnly date, int trainingId)
        {
            var existingTraining = await GetByOrganizationAndDateAsync(organizationId, date, trainingId);
            return existingTraining == null;
        }

        public async Task CreateTrainingAsync(Training training)
        {
            if (await CanCreateTrainingAsync(training.OrganizationId, training.DateOfTraining, training.TrainingId))
            {

                foreach (var employee in training.Employees)
                {
                    _context.Entry(employee).State = EntityState.Unchanged;
                }

                await _context.Trainings.AddAsync(training);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Cannot create multiple trainings for the same organization on the same day.");
            }
        }

        public async Task DeleteTrainingAsync(int id)
        {
            var training = await GetByIdAsync(id);
            if (training != null)
            {
                _context.Trainings.Remove(training);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Training>> GetAllWithDetailsAsync()
        {
            return await _context.Trainings.Where(t => t.IsDeleted == false)
           .Include(t => t.Organization).Where(t => t.Organization.IsDeleted == false)
           .Include(t => t.Employees).Where(t => t.Employees.Any(e => e.IsDeleted == false))
           .ToListAsync();
        }

        public async Task<Training?> GetByIdAsync(int id)
        {
            return await _context.Trainings
           .Include(t => t.Organization).Where(t => t.Organization.IsDeleted == false)
           .Include(t => t.Employees).Where(t => t.Employees.Any(e => e.IsDeleted == false))
           .FirstOrDefaultAsync(t => t.TrainingId == id);
        }

        public async Task<Training?> GetByOrganizationAndDateAsync(int organizationId, DateOnly date, int trainingId)
        {
            return await _context.Trainings.Where(t => t.TrainingId != trainingId)
            .FirstOrDefaultAsync(t => t.OrganizationId == organizationId && t.DateOfTraining == date);

            //return  await _context.Trainings
            //.Where(t => t.TrainingId != trainingId) 
            //.AnyAsync(t => t.OrganizationId == organizationId
            //       && t.DateOfTraining == date
            //       && t.Employees.Any(e => e.EmployeeId.(e.EmployeeId))); 
        }
        public async Task UpdateTrainingAsync(Training training)
        {
            _context.Trainings.Update(training);
            await _context.SaveChangesAsync();
        }
    }
}
