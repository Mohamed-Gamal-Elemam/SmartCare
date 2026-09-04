namespace SmartCare.Services.Abstraction.Exceptions;

public abstract class SmartCareException : Exception
{
    protected SmartCareException(string message) : base(message)
    {
    }
}