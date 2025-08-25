namespace ITC.Authorization.Exceptions;

public class WrongPayloadException : Exception
{
    public WrongPayloadException(string expectedType) : base($"Expected type {expectedType}")
    {

    }
}