using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainVault.DataAccess;
using TrainVault.Interfaces;
using TrainVault.Models;

namespace TrainVault.Repositories
{
	public class OrganizationRepository : IOrganization
	{
		private readonly TrainVaultContext _context;
		public OrganizationRepository(TrainVaultContext context)
		{
			_context = context;
		}

		//adding the organization
		public async Task<OrganizationModel> AddOrganization(OrganizationModel org)
		{
			Organization newOrganization = new Organization()
			{
				OrganizationId = org.OrganizationId,
				OrganizationName = org.OrganizationName,
				Address = org.Address,
				City = org.City,
				State = org.State,
				Country = org.Country,
				Phone = org.Phone,
				IsDeleted = false,
			};

			await _context.Organizations.AddAsync(newOrganization);
			await _context.SaveChangesAsync();

			return org;
		}

		//delete action
		async Task<Organization> IOrganization.DeleteOrganization(int id)
		{
			var organization = await _context.Organizations.FindAsync(id);

			if (organization == null)
			{
				return null;
			}

			// Mark the organization as deleted
			organization.IsDeleted = true;

			// Save changes to the database
			await _context.SaveChangesAsync();

			return organization;
		}


		//getting individual organization
		async Task<Organization> IOrganization.GetOrganizationById(int id)
		{
			var organization = await _context.Organizations.FindAsync(id);

			return organization;

		}

		//get all the organization
		public async Task<List<Organization>> GetOrganizations()
		{
			var organizations = await _context.Organizations.Where(org => org.IsDeleted == false).ToListAsync();
			return organizations;
		}

		public async Task<OrganizationModel> UpdateOrganization(int id, OrganizationModel org)
		{
			Organization newOrganization = new Organization()
			{
				OrganizationId = org.OrganizationId,
				OrganizationName = org.OrganizationName,
				Address = org.Address,
				City = org.City,
				State = org.State,
				Country = org.Country,
				Phone = org.Phone,
				IsDeleted = false,
			};
			_context.Entry(newOrganization).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return org;

		}

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync()
        {
            return await _context.Organizations.Where(o => o.IsDeleted == false)
                .Select(o => new SelectListItem
                {
                    Value = o.OrganizationId.ToString(),
                    Text = o.OrganizationName,
                })
                .ToListAsync();
        }
    }
}
