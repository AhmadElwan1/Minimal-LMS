using Application;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure;
using LiMS.API.Middlewares;
using LiMS.API.Models;
using LiMS.API.Routes;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRepository<Book>, BookRepository>();
builder.Services.AddScoped<IRepository<Member>, MemberRepository>();
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

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<CustomExceptionMiddleware>();

app.MapBookRoutes();
app.MapMemberRoutes();
app.MapBorrowRoutes();

app.Run();