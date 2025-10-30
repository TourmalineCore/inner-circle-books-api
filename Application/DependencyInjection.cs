using Application.Commands;
using Application.Queries;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    private const string DefaultConnection = "DefaultConnection";

    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnection);

        services.AddDbContext<AppDbContext>(options => {
            options.UseNpgsql(connectionString);
        });
        services.AddTransient<CreateBookCommand>();
        services.AddTransient<EditBookCommand>();
        services.AddTransient<DeleteBookCommand>();
        services.AddTransient<DeleteBookCopyCommand>();
        services.AddTransient<DeleteBookCopyReadingHistoryCommand>();
        services.AddTransient<SoftDeleteBookCommand>();
        services.AddTransient<GetBookByIdQuery>();
        services.AddTransient<GetBookByCopyIdQuery>();
        services.AddTransient<GetBookHistoryByIdQuery>();
        services.AddTransient<GetAllBooksQuery>();
        services.AddTransient<GetBookCopyReadingHistoryByCopyIdQuery>();
        services.AddTransient<BookCopyValidatorQuery>();
        services.AddTransient<TakeBookCommand>();
        services.AddTransient<ReturnBookCommand>();
        services.AddTransient<IInnerCircleHttpClient, InnerCircleHttpClient>();
        services.AddTransient<TakeBookService>();
    }
}