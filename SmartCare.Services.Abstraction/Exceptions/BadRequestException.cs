namespace SmartCare.Services.Abstraction.Exceptions;

public class BadRequestException : SmartCareException
{
    public BadRequestException(string message) : base(message)
    {
    }
}