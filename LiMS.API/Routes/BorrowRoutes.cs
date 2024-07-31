using Application;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using LiMS.API.Models;

namespace LiMS.API.Routes
{
    public static class BorrowRoutes
    {
        public static void MapBorrowRoutes(this WebApplication app)
        {
            app.MapPost("/borrow", (BorrowReturnModel request, LibraryService libraryService, IValidator<BorrowReturnModel> validator) =>
            {
                // Validate request
                ValidationResult validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                // Validate Member ID
                if (!request.MemberId.HasValue)
                {
                    return Results.BadRequest(new { Message = "You must provide a Member ID to borrow a book." });
                }

                // Check if the book exists
                if (!libraryService.BookExists(request.BookId))
                {
                    return Results.NotFound(new { Message = "Book not found with the provided Book ID." });
                }

                // Check if the member exists
                if (!libraryService.MemberExists(request.MemberId.Value))
                {
                    return Results.NotFound(new { Message = "Member not found with the provided Member ID." });
                }

                try
                {
                    // Perform the borrowing operation
                    libraryService.BorrowBook(request.BookId, request.MemberId.Value);
                    return Results.Ok(new { Message = "Book borrowed successfully!" });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error borrowing book: {ex.Message}");
                    return Results.Problem("An error occurred while borrowing the book.");
                }
            })
            .WithTags("Borrow");

            app.MapPost("/return", (BorrowReturnModel request, LibraryService libraryService, IValidator<BorrowReturnModel> validator) =>
            {
                // Validate request
                ValidationResult validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                // Check if the book exists
                if (!libraryService.BookExists(request.BookId))
                {
                    return Results.NotFound(new { Message = "Book not found." });
                }

                try
                {
                    // Perform the return operation
                    libraryService.ReturnBook(request.BookId);
                    return Results.Ok(new { Message = "Book returned successfully!" });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error returning book with ID {request.BookId}: {ex.Message}");
                    return Results.Problem("An error occurred while returning the book.");
                }
            })
            .WithTags("Borrow");

            app.MapGet("/borrowed", (LibraryService libraryService) =>
            {
                try
                {
                    List<Book> borrowedBooks = libraryService.GetAllBorrowedBooks();
                    return Results.Ok(borrowedBooks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving borrowed books: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving borrowed books.");
                }
            })
            .WithTags("Borrow");
        }
    }
}
