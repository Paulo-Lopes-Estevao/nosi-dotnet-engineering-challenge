namespace NOS.Engineering.Challenge.Exceptions;

public class GenreNotFoundException : Exception
{
    public GenreNotFoundException()
    {
    }
    
    public GenreNotFoundException(string message) : base(message)
    {
    }

    public GenreNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}