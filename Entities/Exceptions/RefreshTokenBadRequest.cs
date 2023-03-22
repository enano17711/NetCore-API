namespace Entities.Exceptions;

public sealed class RefreshTokenBadRequest : BadRequestException
{
    public RefreshTokenBadRequest() : base("Invalid client request. The tokenDto has same invalid values.")
    {
    }
}