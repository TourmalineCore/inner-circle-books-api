namespace Api.Exceptions;

public class ErrorHandlerMiddleware
{
  private readonly RequestDelegate _next;

  public ErrorHandlerMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Unhandled Exception: {ex}");

      context.Response.StatusCode = StatusCodes.Status500InternalServerError;
      await context.Response.WriteAsJsonAsync(new ExceptionResponse<object>
      {
        Success = false,
        Message = $"An error occurred while processing your request. See the message: {ex.Message}"
      });
    }
  }
}
