using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainVault.DataAccess;
using TrainVault.Interfaces;
using TrainVault.Models;

namespace TrainVault.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployee _employee;
        private readonly IOrganization _organization;
        public EmployeeController(IEmployee employee, IOrganization organization)
        {
            _employee = employee;
            _organization = organization;
        }

        //Index Action
        //public async Task<IActionResult> Index()
        //{
        //    var allEmployee = await _employee.GetEmployees();
        //    return View(allEmployee);
        //}

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.OrganizationSortParm = sortOrder == "Organization" ? "organization_desc" : "Organization";

            var employees = from e in await _employee.GetEmployees()
                            select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                employees = employees.Where(e => e.FirstName.ToLower().Contains(searchString)
                                       || e.LastName.ToLower().Contains(searchString)
                                       || e.Organization.OrganizationName.ToLower().Contains(searchString));
            }

            employees = sortOrder switch
            {
                "name_desc" => employees.OrderByDescending(e => e.FirstName).ThenBy(e => e.LastName),
                "Organization" => employees.OrderBy(e => e.Organization.OrganizationName),
                "organization_desc" => employees.OrderByDescending(e => e.Organization.OrganizationName),
                _ => employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName),
            };

            return View(employees);
        }


        //Creat Actions
        [HttpGet]
        public async  Task<IActionResult> Create()
        {
            // Fetch organizations from the database
            var organizations = await _organization.GetOrganizations();

            // Pass the organizations to the view using ViewBag
            ViewBag.OrganizationId = new SelectList(organizations, "OrganizationId", "OrganizationName");
            return View();
        }
        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreateEmployee(EmployeeModel emp)
        {
            if (emp == null)
            {
                return BadRequest();
            }
            else
            {
                await _employee.AddEmployee(emp);
            }
            TempData["success"] = "Employee data added Successfully";
            return RedirectToAction("Index");
        }

        //Edit Actions
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentEmployee = await _employee.GetEmployeeById(id);

            // Fetch organizations from the database
            var organizations = await _organization.GetOrganizations();

            // Pass the organizations to the view using ViewBag
            ViewBag.OrganizationId = new SelectList(organizations, "OrganizationId", "OrganizationName");

            EmployeeModel emp = new EmployeeModel()
            {
                EmployeeId = currentEmployee.EmployeeId,
                OrganizationId = currentEmployee.OrganizationId,
                FirstName = currentEmployee.FirstName,
                LastName = currentEmployee.LastName,
                Email = currentEmployee.Email,
                Phone = currentEmployee.Phone,
                JobTitle = currentEmployee.JobTitle,
            };
            return View(emp);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> EditEmployee(int id, EmployeeModel emp)
        {
            if (emp == null || id != emp.EmployeeId)
            {
                return NotFound();
            }
            else
            {
                await _employee.UpdateEmployee(id, emp);

            }
            TempData["success"] = "Employee data Updated Successfully";
            return RedirectToAction("Index");
        }


        //Delete Actions

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _employee.GetEmployeeById(id);
            return View(emp);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _employee.DeleteEmployee(id);
            TempData["success"] = "Employee data deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
