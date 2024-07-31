using Application;
using Domain;
using Domain.DTOs;
using FluentValidation.AspNetCore;
using Infrastructure;
using LiMS.API.Models;
using LiMS.API.Routes;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapBookRoutes();
app.MapMemberRoutes();
app.MapBorrowRoutes();

app.Run();