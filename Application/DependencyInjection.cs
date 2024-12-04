using Application.Commands;
using Application.Commands.Contracts;
using Application.Queries;
using Application.Queries.Contracts;
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

        services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString,
                    o => o.UseNodaTime()
                );
            }
        );
        services.AddTransient<ToDoService>();
        services.AddTransient<ICreateToDoCommand, CreateToDoCommand>();
        services.AddTransient<IDeleteToDoCommand, DeleteToDoCommand>();
        services.AddTransient<ISoftDeleteToDoCommand, SoftDeleteToDoCommand>();
        services.AddTransient<IGetToDoByIdQuery, GetToDoByIdQuery>();
        services.AddTransient<IGetAllToDosQuery, GetAllToDosQuery>();
    }
}