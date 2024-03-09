namespace Game.Exceptions;
public class MatchNotFoundException : Exception
{
    public MatchNotFoundException(string message) : base(message)
    {
    }

    public MatchNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class InvalidMoveException : Exception
{
    public InvalidMoveException(string message) : base(message)
    {
    }

    public InvalidMoveException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class EmptyDeckException : Exception
{
    public EmptyDeckException(string message) : base(message)
    {
    }

    public EmptyDeckException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
