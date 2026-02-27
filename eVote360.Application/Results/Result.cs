namespace eVote360.Application.Results;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;
    public List<string> Errors { get; protected set; } = new();

    protected Result(bool isSuccess, string message = "")
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success(string message = "") => new(true, message);
    public static Result Failure(string message) => new(false, message);
    public static Result Failure(List<string> errors) => new Result(false) { Errors = errors };
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool isSuccess, T? data = default, string message = "") : base(isSuccess, message)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string message = "") => new(true, data, message);
    public new static Result<T> Failure(string message) => new(false, default, message);
    public new static Result<T> Failure(List<string> errors) => new Result<T>(false) { Errors = errors };
}
