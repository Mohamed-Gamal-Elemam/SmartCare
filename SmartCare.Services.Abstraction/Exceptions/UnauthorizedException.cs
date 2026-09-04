namespace SmartCare.Services.Abstraction.Exceptions;

public class UnauthorizedException : SmartCareException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}