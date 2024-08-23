using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainVault.DataAccess;
using TrainVault.Interfaces;
using TrainVault.Models;
using TrainVault.Repositories;

namespace TrainVault.Controllers
{
	public class OrganizationController : Controller
	{
		private readonly IOrganization _organization;
		private readonly TrainVaultContext _context;
		public OrganizationController(IOrganization organization)
		{
			_organization = organization;
		}

        //Index Action
        //public async Task<IActionResult> Index()
        //{
        //	var allOrganizations = await _organization.GetOrganizations();
        //	return View(allOrganizations);
        //}

        //sorting index action
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CitySortParm = sortOrder == "City" ? "city_desc" : "City";
            ViewBag.CurrentFilter = searchString;

            var organizations = from o in await _organization.GetOrganizations()
                                where !o.IsDeleted // Exclude deleted organizations
                                select o;

            if (!String.IsNullOrEmpty(searchString))
            {
                organizations = organizations.Where(o => o.OrganizationName.ToLower().Contains(searchString)
                                       || o.City.ToLower().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    organizations = organizations.OrderByDescending(o => o.OrganizationName);
                    break;
                case "City":
                    organizations = organizations.OrderBy(o => o.City);
                    break;
                case "city_desc":
                    organizations = organizations.OrderByDescending(o => o.City);
                    break;
                default:
                    organizations = organizations.OrderBy(o => o.OrganizationName);
                    break;
            }

            return View(organizations);
        }


        //Creat Actions
        [HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		[ActionName("Create")]
		public async Task<IActionResult> CreateOrganization(OrganizationModel org)
		{
			if (org == null)
			{
				return BadRequest();
			}
			else
			{
				await _organization.AddOrganization(org);
			}
			TempData["success"] = "Organization added Successfully";
			return RedirectToAction("Index");
		}

		//Edit Actions
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var currentOrganization = await _organization.GetOrganizationById(id);
			OrganizationModel org = new OrganizationModel()
			{
				OrganizationId = currentOrganization.OrganizationId,
				OrganizationName = currentOrganization.OrganizationName,
				Address = currentOrganization.Address,
				City = currentOrganization.City,
				State = currentOrganization.State,
				Country = currentOrganization.Country,
				Phone = currentOrganization.Phone,
				IsDeleted = currentOrganization.IsDeleted,
			};
			return View(org);
		}

		[HttpPost]
		[ActionName("Edit")]
		public async  Task<IActionResult> EditOrganization(int id, OrganizationModel org)
		{
			if (org == null || id != org.OrganizationId)
			{
				return NotFound();
			}
			else
			{
				await _organization.UpdateOrganization(id, org);

			}
            TempData["success"] = "Organization Updated Successfully";
            return RedirectToAction("Index");
		}


		//Delete Actions

		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			var org = await _organization.GetOrganizationById(id);
			return View(org);
		}

		[HttpPost]
		[ActionName("Delete")]
		public async Task<IActionResult> DeleteOrganization(int id)
		{
            await _organization.DeleteOrganization(id);
            TempData["success"] = "Organization deleted Successfully";
            return RedirectToAction("Index");
        }
	}
}
