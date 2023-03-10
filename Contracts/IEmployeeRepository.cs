using Entities.Models;

namespace Contracts;

public interface IEmployeeRepository
{
    IEnumerable<Employee> GetAllEmployees(Guid companyId, bool trackChanges);
}