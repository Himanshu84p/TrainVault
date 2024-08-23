using TrainVault.DataAccess;
using TrainVault.Models;

namespace TrainVault.Interfaces
{
    public interface ITraining
    {
		Task<Training?> GetByIdAsync(int id);
		Task<IEnumerable<Training>> GetAllWithDetailsAsync();
		Task CreateTrainingAsync(Training training);
		Task UpdateTrainingAsync(Training training);
		Task DeleteTrainingAsync(int id);
		Task<bool> CanCreateTrainingAsync(int organizationId, DateOnly date, int trainingId);
		Task<Training?> GetByOrganizationAndDateAsync(int organizationId, DateOnly date, int trainingId);
	}
}
