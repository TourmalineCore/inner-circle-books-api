using Application.Commands;
using Application.Commands.Contracts;
using Application.Queries;
using Application.Queries.Contracts;
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

        services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(connectionString); }
        );
        services.AddTransient<ICreateBookCommand, CreateBookCommand>();
        services.AddTransient<IUpdateBookCommand, UpdateBookCommand>();
        services.AddTransient<IDeleteBookCommand, DeleteBookCommand>();
        services.AddTransient<ISoftDeleteBookCommand, SoftDeleteBookCommand>();
        services.AddTransient<IGetBookByIdQuery, GetBookByIdQuery>();
        services.AddTransient<IGetAllBooksQuery, GetAllBooksQuery>();

        services.AddTransient<ICreateAuthorCommand, CreateAuthorCommand>();
        services.AddTransient<IDeleteAuthorCommand, DeleteAuthorCommand>();
        services.AddTransient<IUpdateAuthorCommand, UpdateAuthorCommand>();
    }
}