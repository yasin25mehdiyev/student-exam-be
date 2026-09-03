namespace StudentExam.Application.Common.Exceptions;

/// <summary>
/// Thrown by the Infrastructure layer when a database operation fails due to a
/// foreign key constraint, so the Application layer never needs to depend on EF Core's
/// own exception types.
/// </summary>
public class ForeignKeyConstraintException : Exception
{
    public ForeignKeyConstraintException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
