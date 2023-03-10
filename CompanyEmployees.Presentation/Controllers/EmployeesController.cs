using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public EmployeesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET
    [HttpGet]
    public IActionResult GetEmployeesFroCompany(Guid companyId)
    {
        var employees = _serviceManager.EmployeeService.GetEmployees(companyId,
            trackChanges: false);
        return Ok(employees);
    }
}