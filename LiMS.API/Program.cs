using Application;
using Domain;
using Infrastructure;
using LiMS.API.Routes;

var builder = WebApplication.CreateBuilder(args);

// Register services and repositories
builder.Services.AddSingleton<IRepository<Book>>(new BookRepository());
builder.Services.AddSingleton<IRepository<Member>>(new MemberRepository());
builder.Services.AddSingleton<LibraryService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

// Map endpoints
app.MapBookRoutes();
app.MapMemberRoutes();
app.MapBorrowRoutes();

app.Run();

namespace LiMS.API
{
    public class BorrowRequest
    {
        public int BookId { get; set; }
        public int MemberId { get; set; }
    }
}