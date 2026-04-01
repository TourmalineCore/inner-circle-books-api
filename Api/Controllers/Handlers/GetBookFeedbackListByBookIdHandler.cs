using Api.Responses;
using Application;
using Application.Queries;

namespace Api.Controllers.Handlers;

public class GetBookFeedbackListByBookIdHandler
{
    private readonly IInnerCircleHttpClient _client;

    private readonly GetBookFeedbackListByBookIdQuery _getBookFeedbackListByBookIdQuery;
    
    public GetBookFeedbackListByBookIdHandler(
        IInnerCircleHttpClient client,
        GetBookFeedbackListByBookIdQuery getBookFeedbackListByBookIdQuery
    )
    {
        _client = client;
        _getBookFeedbackListByBookIdQuery = getBookFeedbackListByBookIdQuery;
    }

    public async Task<BooksFeedbackListResponse> HandleAsync(long bookId, long tenantId)
    {
        var bookFeedback = await _getBookFeedbackListByBookIdQuery.GetAsync(bookId, tenantId);

        var employeesIds = bookFeedback
            .Select(x => x.EmployeeId)
            .ToList();

        var employeesByIds = await _client.GetEmployeesByIdsAsync(employeesIds);

        return new BooksFeedbackListResponse
        {
            BookFeedbackList = bookFeedback
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
            FeedbackCount = bookFeedback.Count
        };
    }
}
