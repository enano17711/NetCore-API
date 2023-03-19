using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.extensions;
using Shared.RequestFeatures;

namespace Repository;

public sealed class EmployeeRepository : RepositoryBase<Employee>,
    IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<PagedList<Employee>> GetAllEmployeesAsync(Guid companyId,
        EmployeeParameters employeeParameters,
        bool trackChanges)
    {
        var employees = await FindByCondition(e => e.CompanyId.Equals(companyId),
                trackChanges)
            .FilterEmployees(employeeParameters.MinAge,
                employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm)
            .OrderBy(e => e.Name)
            .ToListAsync();

        return PagedList<Employee>.ToPagedList(employees,
            employeeParameters.PageNumber,
            employeeParameters.PageSize);

        // this is for more performance (call to database here)
        // var employees = await FindByCondition(e => e.CompanyId.Equals(companyId),
        //         trackChanges)
        //     .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
        //     .Search(employeeParameters.SearTerm)
        //     .OrderBy(e => e.Name)
        //     .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
        //     .Take(employeeParameters.PageSize)
        //     .ToListAsync();
        //
        // var count = await FindByCondition(e => e.CompanyId.Equals(companyId),
        //         trackChanges)
        //     .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
        //     .Search(employeeParameters.SearTerm)
        //     .CountAsync();
        //
        // return new PagedList<Employee>(employees,
        //     count,
        //     employeeParameters.PageNumber,
        //     employeeParameters.PageSize);
    }

    public async Task<Employee?> GetEmployeeAsync(Guid companyId,
        Guid employeeId,
        bool trackChanges) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(employeeId),
                trackChanges)
            .SingleOrDefaultAsync();

    public void CreateEmployeeForCompany(Guid companyId,
        Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public void DeleteEmployee(Employee employee) =>
        Delete(employee);
}