using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;

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
    public IActionResult GetEmployeesFromCompany(Guid companyId)
    {
        var employees = _serviceManager.EmployeeService.GetEmployees(companyId,
            trackChanges: false);
        return Ok(employees);
    }

    [HttpGet("{id:guid}",
        Name = "GetEmployeeForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId,
        Guid id)
    {
        var employee = _serviceManager.EmployeeService.GetEmployee(companyId,
            id,
            trackChanges: false);
        return Ok(employee);
    }

    // POST
    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId,
        [FromBody] EmployeeForCreationDto employee)
    {
        if (employee is null)
            return BadRequest("EmployeeForCreationDto object is null");

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("CustomError", "Este es un error personalizado");
            return UnprocessableEntity(ModelState);
        }

        var employeeToReturn =
            _serviceManager.EmployeeService.CreateEmployeeForCompany(companyId,
                employee,
                trackChanges: false);
        return CreatedAtRoute("GetEmployeeForCompany",
            new
            {
                companyId,
                id = employeeToReturn.Id
            },
            employeeToReturn);
    }

    // DELETE
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployeeForCompany(Guid companyId,
        Guid id)
    {
        _serviceManager.EmployeeService.DeleteEmployeeForCompany(companyId,
            id,
            trackChanges: false);
        return NoContent();
    }

    // UPDATE
    [HttpPut("{id:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId,
        Guid id,
        [FromBody] EmployeeForUpdateDto employee)
    {
        if (employee is null)
            return BadRequest("EmployeeForUpdateDto object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        _serviceManager.EmployeeService.UpdateEmployeeForCompany(companyId,
            id,
            employee,
            compTrackChanges: false,
            empTrackChanges: true);
        return NoContent();
    }

    // PATCH
    [HttpPatch("{id:guid}")]
    public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId,
        Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null.");

        var result =
            _serviceManager.EmployeeService.GetEmployeeForPatch(companyId,
                id,
                compTrackChanges: false,
                empTrackChanges: true);

        patchDoc.ApplyTo(result.employeeToPatch, ModelState);

        TryValidateModel(result.employeeToPatch);
        
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);


        _serviceManager.EmployeeService.SaveChangesForPatch(result.employeeToPatch,
            result.employeeEntity);
        return NoContent();
    }
}