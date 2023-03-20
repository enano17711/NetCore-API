using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Utility;

public class EmployeeLinks : IEmployeeLinks
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<EmployeeDto> _dataShaper;
    /// <summary>
    /// Constructor for EmployeeLinks
    /// </summary>
    /// <param name="linkGenerator">Link generator</param>
    /// <param name="dataShaper">DataShaper interface</param>
    public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }

    /// <summary>
    /// Generates links for the given EmployeeDto. This is the method that will be called by the Web Api
    /// </summary>
    /// <param name="employeesDto">The DTO of the Employee</param>
    /// <param name="fields">The fields to return in the links. If null all fields are returned</param>
    /// <param name="companyId">The id of the company for which the links are generated</param>
    /// <param name="httpContext">The HttpContext for the current request ( for example Request Session</param>
    /// <returns>Collection of only shaped data or with links</returns>
    public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string fields, Guid companyId,
        HttpContext httpContext)
    {
        var shapedEmployees = ShapeData(employeesDto, fields);

        // Returns a list of employees that should be linked to the employee.
        if (ShouldGenerateLinks(httpContext))
            return ReturnLinkedEmployees(employeesDto, fields, companyId, httpContext, shapedEmployees);

        return ReturnShapedEmployees(shapedEmployees);
    }

    /// <summary>
    /// Returns shape data for the list of EmployeeDto. This is a wrapper around data shaper.
    /// </summary>
    /// <param name="employeesDto">Enumeration of DTOs to shape</param>
    /// <param name="fields">Comma separated list of fields</param>
    /// <returns>Shape data</returns>
    private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDto, string fields)
        =>
            _dataShaper.ShapeData(employeesDto, fields)
                .Select(e => e.Entity)
                .ToList();

    /// <summary>
    /// Determines whether links should be generated. This is determined by the media type of the request. If it ends with hateoas we don't generate links
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request.</param>
    /// <returns>A value indicating whether links should be generated or not. True if the link generation is needed ; otherwise false</returns>
    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
            StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Returns shaped employment entities. This is a wrapper to allow the caller to specify the list of entities that need to be returned in the response
    /// </summary>
    /// <param name="shapedEmployees">List of entities to</param>
    /// <returns>Shape data</returns>
    private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees) =>
        new LinkResponse { ShapedEntities = shapedEmployees };

    /// <summary>
    /// Returns a list of links to the employees. It is used to create the view for the linked Employees
    /// </summary>
    /// <param name="employeesDto">The IEnumerable of Employees DTO</param>
    /// <param name="fields">The fields to return for each link. eg.</param>
    /// <param name="companyId">The guid of the company. If null the GUID will be auto - generated.</param>
    /// <param name="httpContext">The HttpContext for the current request.</param>
    /// <param name="shapedEmployees">The list of Shaped Entity</param>
    /// <returns>A list of links to the employees</returns>
    private LinkResponse ReturnLinkedEmployees(IEnumerable<EmployeeDto> employeesDto,
        string fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
    {
        var employeeDtoList = employeesDto.ToList();
        // Create links for all employees in employeeDtoList
        for (var index = 0; index < employeeDtoList.Count(); index++)
        {
            var employeeLinks = CreateLinksForEmployee(httpContext, companyId,
                employeeDtoList[index].Id, fields);
            shapedEmployees[index].Add("Links", employeeLinks);
        }

        var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
        var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);
        return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
    }

    /// <summary>
    /// Creates the links for employee. Note that this is the same as CreateEmployeeForCompany but with the addition of links that get deleted and updated in the web service
    /// </summary>
    /// <param name="httpContext">The http context for the web service</param>
    /// <param name="companyId">The identifier of the company to link to</param>
    /// <param name="id">The identifier of the employee to link to</param>
    /// <param name="fields">The fields to return for the link ( default : " employee_id "</param>
    /// <returns>The links for the employee</returns>
    private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId,
        Guid id, string fields = "")
    {
        var links = new List<Link>
        {
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany",
                    values: new { companyId, id, fields }),
                "self",
                "GET"),
            new Link(_linkGenerator.GetUriByAction(httpContext,
                    "DeleteEmployeeForCompany", values: new { companyId, id }),
                "delete_employee",
                "DELETE"),
            new Link(_linkGenerator.GetUriByAction(httpContext,
                    "UpdateEmployeeForCompany", values: new { companyId, id }),
                "update_employee",
                "PUT"),
            new Link(_linkGenerator.GetUriByAction(httpContext,
                    "PartiallyUpdateEmployeeForCompany", values: new { companyId, id }),
                "partially_update_employee",
                "PATCH")
        };
        return links;
    }

    /// <summary>
    /// Creates links for GetEmployeesForCompany action. Gets the list of all employees that belong to the current company
    /// </summary>
    /// <param name="httpContext">The for the current request</param>
    /// <param name="employeesWrapper">The wrapper for the Employees</param>
    /// <returns>The links for the Employees</returns>
    private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext,
        LinkCollectionWrapper<Entity> employeesWrapper)
    {
        employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,
                "GetEmployeesForCompany", values: new { }),
            "self",
            "GET"));
        return employeesWrapper;
    }
}