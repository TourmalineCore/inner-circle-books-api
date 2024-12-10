namespace Api.Exceptions;

public class ExceptionResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}