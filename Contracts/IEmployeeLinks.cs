using Entities.LinkModels;
using Shared.DataTransferObjects;
using Microsoft.AspNetCore.Http;

namespace Contracts;

public interface IEmployeeLinks
{
    /// <summary>
    /// Tries to generate links. This is used by the link generator to determine if links should be generated or not
    /// </summary>
    /// <param name="employeesDto">Employee DTO that contains information about the employees</param>
    /// <param name="fields">Comma separated list of fields to link</param>
    /// <param name="companyId">Company GUID that the link should point to</param>
    /// <param name="httpContext">HttpContext for the current request ( for example Request Response )</param>
    LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string fields, Guid companyId,
        HttpContext httpContext);
}