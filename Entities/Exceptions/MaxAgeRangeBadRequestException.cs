namespace Entities.Exceptions;

public sealed class MaxAgeRangeBadRequestException : BadRequestException
{
    public MaxAgeRangeBadRequestException() : base("Max age range cannot be less than min age.")
    {
    }
}