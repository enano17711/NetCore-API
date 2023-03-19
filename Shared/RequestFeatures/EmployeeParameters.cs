namespace Shared.RequestFeatures;

public class EmployeeParameters : RequestParameters
{
    public uint MinAge { get; set; }
    public uint MaxAge { get; set; } = UInt32.MaxValue;

    public bool ValidAgeRange => MaxAge > MinAge;

    public string? SearchTerm { get; set; }
}