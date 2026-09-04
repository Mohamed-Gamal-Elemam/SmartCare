namespace SmartCare.Services.Abstraction.Exceptions;

public class NotFoundException : SmartCareException
{
    public NotFoundException(string message) : base(message)
    {
    }
}