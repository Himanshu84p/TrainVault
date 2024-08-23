using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TrainVault.DataAccess;
using TrainVault.Interfaces;
using TrainVault.Models;
using TrainVault.Repositories;

namespace TrainVault.Controllers
{
    public class TrainingController : Controller
    {
        private readonly ITraining _training;
        private readonly IEmployee _employee;
        private readonly IOrganization _organization;
        public TrainingController(ITraining training ,IEmployee employee, IOrganization organization)
        {
            _training = training;
            _employee = employee;
            _organization = organization;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["OrgSortParm"] = sortOrder == "Org" ? "org_desc" : "Org";
            ViewData["CurrentFilter"] = searchString;

            var trainings = from t in await _training.GetAllWithDetailsAsync()
                            select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                trainings = trainings.Where(t => t.Organization.OrganizationName.ToLower().Contains(searchString.ToLower())
                                               || t.PurposeOfTraining.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    trainings = trainings.OrderByDescending(t => t.DateOfTraining);
                    break;
                case "Org":
                    trainings = trainings.OrderBy(t => t.Organization.OrganizationName);
                    break;
                case "org_desc":
                    trainings = trainings.OrderByDescending(t => t.Organization.OrganizationName);
                    break;
                default:
                    trainings = trainings.OrderBy(t => t.DateOfTraining);
                    break;
            }

            return View(trainings);
        }


        public async Task<IActionResult> Create()
		{
			var viewModel = new TrainingModel
			{
				Organizations = await _organization.GetAllAsSelectListItemsAsync(),
				Employees = new List<SelectListItem>()
			};
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(TrainingModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				viewModel.Organizations = await _organization.GetAllAsSelectListItemsAsync();
				viewModel.Employees = await _employee.GetEmployeesByOrganizationAsync(viewModel.OrganizationId);
				return View(viewModel);
			}

            //if no employees selected then throw error
            if (viewModel.SelectedEmployeeIds.Count() == 0)
            {
                ModelState.AddModelError(string.Empty, "Employees can not be empty!!");
                viewModel.Organizations = await _organization.GetAllAsSelectListItemsAsync();
                viewModel.Employees = await _employee.GetEmployeesByOrganizationAsync(viewModel.OrganizationId);
                return View(viewModel);
            }

            var training = new Training
			{
				DateOfTraining = viewModel.DateOfTraining,
				PurposeOfTraining = viewModel.Purpose,
				PlaceOfTraining = viewModel.Place,
				OrganizationId = viewModel.OrganizationId,
				IsDeleted = false,
				Employees = viewModel.SelectedEmployeeIds.Select(id => new Employee { EmployeeId = id }).ToList()
			};


            //checks if training can be create or not
            if (!await _training.CanCreateTrainingAsync(training.OrganizationId, training.DateOfTraining, training.TrainingId))
            {
                ModelState.AddModelError(string.Empty, "A training session for this organization already exists on the selected date.");
                viewModel.Organizations = await _organization.GetAllAsSelectListItemsAsync();
				viewModel.Employees = await _employee.GetEmployeesByOrganizationAsync(viewModel.OrganizationId);
                return View(viewModel);
            }
            
            try
			{
                await _training.CreateTrainingAsync(training);
                TempData["success"] = "Training Created Successfully";
                return RedirectToAction("Index");
			}
			catch (InvalidOperationException ex)
			{
				ModelState.AddModelError("", ex.Message);
				viewModel.Organizations = await _organization.GetAllAsSelectListItemsAsync();
				viewModel.Employees = await _employee.GetEmployeesByOrganizationAsync(viewModel.OrganizationId);
				return View(viewModel);
			}
		}

		public async Task<IActionResult> Edit(int id)
		{
			var training = await _training.GetByIdAsync(id);
			if (training == null) return NotFound();

			var viewModel = new TrainingModel
			{
				TrainingId = training.TrainingId,
				DateOfTraining = training.DateOfTraining,
				Purpose = training.PurposeOfTraining,
				Place = training.PlaceOfTraining,
				OrganizationId = training.OrganizationId,
				SelectedEmployeeIds = training.Employees.Select(e => e.EmployeeId).ToList(),
				Organizations = await _organization.GetAllAsSelectListItemsAsync(),
				Employees = await _employee.GetEmployeesByOrganizationAsync(training.OrganizationId)
			};

			return View(viewModel);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingModel viewModel)
        {
            if (id != viewModel.TrainingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!await _training.CanCreateTrainingAsync(viewModel.OrganizationId, viewModel.DateOfTraining, viewModel.TrainingId))
                {
                    ModelState.AddModelError(string.Empty, "A training session for this organization already exists on the selected date.");
                    viewModel.Organizations = await _organization.GetAllAsSelectListItemsAsync();
					
					viewModel.Employees = await _employee.GetEmployeesByOrganizationAsync(viewModel.OrganizationId);


                    return View(viewModel);
                }
                if (viewModel.SelectedEmployeeIds.Count() == 0)
                {
                    ModelState.AddModelError(string.Empty, "Employees can not be empty!!");
                    viewModel.Organizations = await _organization.GetAllAsSelectListItemsAsync();
                    viewModel.Employees = await _employee.GetEmployeesByOrganizationAsync(viewModel.OrganizationId);


                    return View(viewModel);
                }
                try
                {
                    var trainingToUpdate = await _training.GetByIdAsync(id);

                    if (trainingToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Update scalar properties
                    trainingToUpdate.DateOfTraining = viewModel.DateOfTraining;
                    trainingToUpdate.PurposeOfTraining = viewModel.Purpose;
                    trainingToUpdate.PlaceOfTraining = viewModel.Place;
                    trainingToUpdate.OrganizationId = viewModel.OrganizationId;

                    // Clear existing employees and add selected ones
                    trainingToUpdate.Employees.Clear();

                    foreach (var employeeId in viewModel.SelectedEmployeeIds)
                    {
                        var employee = await _employee.GetEmployeeById(employeeId);
                        if (employee != null)
                        {
                            trainingToUpdate.Employees.Add(employee);
                        }
                    }

                    await _training.UpdateTrainingAsync(trainingToUpdate);
                    TempData["success"] = "Training Updated Successfully";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                        return NotFound();
				}
            }

            viewModel.Organizations = await _organization.GetAllAsSelectListItemsAsync();
            viewModel.Employees = await _employee.GetEmployeesByOrganizationAsync(viewModel.OrganizationId);
            return View(viewModel);
        }


        public async Task<IActionResult> Delete(int id)
		{
			var training = await _training.GetByIdAsync(id);
			if (training == null) return NotFound();

			return View(training);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _training.DeleteTrainingAsync(id);
            TempData["success"] = "Training Deleted Successfully";
            return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> GetEmployeesByOrganization(int organizationId)
		{
			var employees = await _employee.GetEmployeesByOrganizationAsync(organizationId);
			return Json(employees.Select(e => new { value = e.Value, text = e.Text}));
		}
	}
}


