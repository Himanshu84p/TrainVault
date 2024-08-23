using Microsoft.AspNetCore.Mvc.Rendering;
using TrainVault.DataAccess;
using TrainVault.Models;

namespace TrainVault.Interfaces
{
    public interface IOrganization
    {
        public Task<List<Organization>> GetOrganizations();

        public Task<OrganizationModel> AddOrganization(OrganizationModel organization);

        public Task<Organization> GetOrganizationById(int id);

        public Task<OrganizationModel> UpdateOrganization(int id, OrganizationModel organization);

        public Task<Organization> DeleteOrganization(int id);

        public Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync();
    }
}
