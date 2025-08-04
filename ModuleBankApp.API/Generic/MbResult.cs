namespace ModuleBankApp.API.Generic;

public class MbResult<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string? Error { get; }

    private MbResult(T value, bool isSuccess, string? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static MbResult<T> Success(T value) => new(value, true, null);
    public static MbResult<T> Failure(string error) => new(default!, false, error);
}