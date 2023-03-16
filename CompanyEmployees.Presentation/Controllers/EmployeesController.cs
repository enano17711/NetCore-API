using CompanyEmployees.Presentation.ActionFilters;
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
    public async Task<IActionResult> GetEmployeesFromCompany(Guid companyId)
    {
        var employees = await _serviceManager.EmployeeService.GetEmployeesAsync(companyId,
            trackChanges: false);
        return Ok(employees);
    }

    // GET

    [HttpGet("{id:guid}",
        Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId,
        Guid id)
    {
        var employee = await _serviceManager.EmployeeService.GetEmployeeAsync(companyId,
            id,
            trackChanges: false);
        return Ok(employee);
    }

    // POST
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId,
        [FromBody] EmployeeForCreationDto employee)
    {
        var employeeToReturn =
            await _serviceManager.EmployeeService.CreateEmployeeForCompanyAsync(companyId,
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
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId,
        Guid id)
    {
        await _serviceManager.EmployeeService.DeleteEmployeeForCompanyAsync(companyId,
            id,
            trackChanges: false);
        return NoContent();
    }

    // UPDATE
    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId,
        Guid id,
        [FromBody] EmployeeForUpdateDto employee)
    {
        await _serviceManager.EmployeeService.UpdateEmployeeForCompanyAsync(companyId,
            id,
            employee,
            compTrackChanges: false,
            empTrackChanges: true);
        return NoContent();
    }

    // PATCH
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId,
        Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null.");

        var result =
            await _serviceManager.EmployeeService.GetEmployeeForPatchAsync(companyId,
                id,
                compTrackChanges: false,
                empTrackChanges: true);

        patchDoc.ApplyTo(result.employeeToPatch, ModelState);

        TryValidateModel(result.employeeToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);


        await _serviceManager.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch,
            result.employeeEntity);
        return NoContent();
    }
}