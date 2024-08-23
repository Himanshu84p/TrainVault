using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainVault.DataAccess;
using TrainVault.Interfaces;
using TrainVault.Models;

namespace TrainVault.Repositories
{
    public class EmployeeRepository : IEmployee
    {
        private readonly TrainVaultContext _context;

        public EmployeeRepository(TrainVaultContext context)
        {
            _context = context;
        }
        public async Task<EmployeeModel> AddEmployee(EmployeeModel employee)
        {
            Employee newEmployee = new Employee()
            {
                EmployeeId = employee.EmployeeId,
                OrganizationId = employee.OrganizationId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                JobTitle = employee.JobTitle,
                IsDeleted = false,
            };

            await _context.Employees.AddAsync(newEmployee);
            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<Employee> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return null;
            }

            // Mark the organization as deleted
            employee.IsDeleted = true;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Organization) 
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            return employee;
        }

        public async Task<List<Employee>> GetEmployees()
        {
            var employees = await _context.Employees.Where(emp => emp.IsDeleted == false).Include(e => e.Organization).ToListAsync();
            return employees;
        }

		

		public async Task<EmployeeModel> UpdateEmployee(int id, EmployeeModel employee)
        {
            Employee newEmployee = new Employee()
            {
                EmployeeId = employee.EmployeeId,
                OrganizationId = employee.OrganizationId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                JobTitle = employee.JobTitle,
                IsDeleted = false,
            };
            _context.Entry(newEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<IEnumerable<SelectListItem>> GetEmployeesByOrganizationAsync(int organizationId)
        {
            return await _context.Employees
                .Where(e => e.OrganizationId == organizationId)
                .Where(e => e.IsDeleted == false)
                .Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = $"{e.FirstName} {e.LastName}"
                })
                .ToListAsync();
        }
    }
}
