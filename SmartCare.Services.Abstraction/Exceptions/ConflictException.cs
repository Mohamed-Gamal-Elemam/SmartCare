namespace SmartCare.Services.Abstraction.Exceptions;

public class ConflictException : SmartCareException
{
    public ConflictException(string message) : base(message)
    {
    }
}