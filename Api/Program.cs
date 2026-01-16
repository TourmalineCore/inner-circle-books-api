using System.Reflection;
using Api;
using Api.Exceptions;
using Application;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers(opt =>
{
  opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
});

builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication(configuration);

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "Books API",
    Description = "An ASP.NET Core Web API for managing books in TourmalineCore office",
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
  });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Description = "Please insert JWT with Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      new string[] { }
    }
  });

  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var authenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>();
builder.Services.Configure<AuthenticationOptions>(configuration.GetSection(nameof(AuthenticationOptions)));
builder.Services
  .AddJwtAuthentication(authenticationOptions)
  .WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.PermissionClaimType);
builder.Services.Configure<InnerCircleServiceUrls>(configuration.GetSection(nameof(InnerCircleServiceUrls)));
builder.Services.AddHttpContextAccessor();

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

app.UseJwtAuthentication();

app.MapControllers();

app.Run();
