namespace NOS.Engineering.Challenge.Exceptions;

public class GenreAlreadyExistsException : Exception
{
    public GenreAlreadyExistsException()
    {
    }

    public GenreAlreadyExistsException(string message) : base(message)
    {
    }

    public GenreAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}