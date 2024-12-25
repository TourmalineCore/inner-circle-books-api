using System.Reflection;
using Api.Exceptions;
using Application;
using Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers(opt => { opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>(); });

builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication(configuration);
builder.Services.AddTransient<IClock, Clock>();


builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Version = "v1",
                Title = "ToDo API",
                Description = "An ASP.NET Core Web API for managing ToDo items",
                Contact = new OpenApiContact
                {
                    Name = "Website",
                    Url = new Uri("https://www.tourmalinecore.com/en")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/license/mit")
                }
            }
        );
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }
);

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors(
    corsPolicyBuilder => corsPolicyBuilder
        .AllowAnyHeader()
        .SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyOrigin()
);

app.UseStaticFiles();

app.UseSwagger();

app.UseSwaggerUI();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();

app.Run();