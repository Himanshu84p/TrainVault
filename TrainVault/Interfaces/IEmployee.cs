using Microsoft.AspNetCore.Mvc.Rendering;
using TrainVault.DataAccess;
using TrainVault.Models;

namespace TrainVault.Interfaces
{
    public interface IEmployee
    {
        public Task<List<Employee>> GetEmployees();

        public Task<EmployeeModel> AddEmployee(EmployeeModel employee);
       
        public Task<Employee> GetEmployeeById(int id);
      
        public Task<EmployeeModel> UpdateEmployee(int id, EmployeeModel employee);

        public Task<Employee> DeleteEmployee(int id);

        Task<IEnumerable<SelectListItem>> GetEmployeesByOrganizationAsync(int organizationId);
    }
}
