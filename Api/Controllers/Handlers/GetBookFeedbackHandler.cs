using Api.Responses;
using Application;
using Application.Queries;

namespace Api.Controllers.Handlers;

public class GetBookFeedbackHandler
{
    private readonly IInnerCircleHttpClient _client;

    private readonly GetBookFeedbackQuery _getBookFeedbackQuery;
    
    public GetBookFeedbackHandler(
        IInnerCircleHttpClient client,
        GetBookFeedbackQuery getBookFeedbackQuery
    )
    {
        _client = client;
        _getBookFeedbackQuery = getBookFeedbackQuery;
    }

    public async Task<GetBookFeedbackResponse> HandleAsync(long bookId, long tenantId)
    {
        var bookFeedback = await _getBookFeedbackQuery.GetAsync(bookId, tenantId);

        var employeesIds = bookFeedback
            .Select(x => x.EmployeeId)
            .ToList();

        var employeesByIds = await _client.GetEmployeesByIdsAsync(employeesIds);

        return new GetBookFeedbackResponse
        {
            BookFeedback = bookFeedback
                .Select(x =>
                {
                    return new BookFeedbackDto
                    {
                        Id = x.Id,
                        EmployeeFullName = employeesByIds.FirstOrDefault(employee => employee.EmployeeId == x.EmployeeId).FullName,
                        LeftFeedbackAtUtc = x.LeftFeedbackAtUtc,
                        ProgressOfReading = x.ProgressOfReading.ToString(),
                        Rating = x.Rating,
                        Advantages = x.Advantages,
                        Disadvantages = x.Disadvantages
                    };
                })
                .ToList(),
        };
    }
}
