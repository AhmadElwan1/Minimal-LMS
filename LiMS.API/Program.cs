using Application;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure;
using LiMS.API.Middlewares;
using LiMS.API.Models;
using LiMS.API.Routes;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<IRepository<Book>, BookRepository>();
builder.Services.AddSingleton<IRepository<Member>, MemberRepository>();
builder.Services.AddScoped<LibraryService>();

builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<BookModel>();
    fv.RegisterValidatorsFromAssemblyContaining<MemberModel>();
    fv.RegisterValidatorsFromAssemblyContaining<BorrowReturnModel>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Register custom exception middleware
app.UseMiddleware<CustomExceptionMiddleware>();

// Swagger setup
app.UseSwagger();
app.UseSwaggerUI();

// Map routes
app.MapBookRoutes();
app.MapMemberRoutes();
app.MapBorrowRoutes();

app.Run();