namespace SmartCare.Services.Abstraction.Exceptions;

public class ForbiddenException : SmartCareException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}