namespace StudentExam.Application.Common;

public enum ServiceErrorType
{
    None,
    NotFound,
    Conflict,
    Validation
}

public class ServiceResult
{
    public bool Succeeded { get; private init; }
    public string? Error { get; private init; }
    public ServiceErrorType ErrorType { get; private init; } = ServiceErrorType.None;

    public static ServiceResult Success() => new() { Succeeded = true };

    public static ServiceResult Fail(string error, ServiceErrorType errorType) =>
        new() { Succeeded = false, Error = error, ErrorType = errorType };
}

public class ServiceResult<T>
{
    public bool Succeeded { get; private init; }
    public T? Data { get; private init; }
    public string? Error { get; private init; }
    public ServiceErrorType ErrorType { get; private init; } = ServiceErrorType.None;

    public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };

    public static ServiceResult<T> Fail(string error, ServiceErrorType errorType) =>
        new() { Succeeded = false, Error = error, ErrorType = errorType };
}
