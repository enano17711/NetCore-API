namespace Shared.DataTransferObjects;

/// <summary>
/// Creates a DTO for Employee with properties specified in the object. This is used to create an instance of the class
/// </summary>
/// <param name="Id">Id of employee</param>
/// <param name="Name">Name of employee</param>
/// <param name="Age">Age of employee</param>
/// <param name="Position">Position of employee</param>
public record EmployeeDto(Guid Id, string Name, int Age, string Position);