using Application;
using Domain;
using Domain.DTOs;
using FluentValidation;

namespace LiMS.API.Routes
{
    public static class BorrowRoutes
    {
        public static void MapBorrowRoutes(this WebApplication app)
        {
            // Route to borrow a book
            app.MapPost("/borrow", async (BorrowRequestDto request, LibraryService libraryService, IValidator<BorrowRequestDto> validator) =>
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    libraryService.BorrowBook(request);
                    return Results.Ok(new { Message = "Book borrowed successfully!" });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    // Log the exception (could use a logging framework here)
                    Console.WriteLine($"Error borrowing book: {ex.Message}");
                    return Results.Problem("An error occurred while borrowing the book.");
                }
            })
            .WithTags("Borrow");

            // Route to return a book
            app.MapPost("/return", async (int bookId, LibraryService libraryService) =>
            {
                try
                {
                    libraryService.ReturnBook(bookId);
                    return Results.Ok(new { Message = "Book returned successfully!" });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    // Log the exception (could use a logging framework here)
                    Console.WriteLine($"Error returning book with ID {bookId}: {ex.Message}");
                    return Results.Problem("An error occurred while returning the book.");
                }
            })
            .WithTags("Borrow");

            // Route to get all borrowed books
            app.MapGet("/borrowed", (LibraryService libraryService) =>
            {
                try
                {
                    List<Book> borrowedBooks = libraryService.GetAllBorrowedBooks();
                    return Results.Ok(borrowedBooks);
                }
                catch (Exception ex)
                {
                    // Log the exception (could use a logging framework here)
                    Console.WriteLine($"Error retrieving borrowed books: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving borrowed books.");
                }
            })
            .WithTags("Borrow");
        }
    }
}
