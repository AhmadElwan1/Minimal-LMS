using Application;
using Domain;
using LiMS.API.Models;
using FluentValidation.Results;

namespace LiMS.API.Routes
{
    public static class BorrowRoutes
    {
        public static void MapBorrowRoutes(this WebApplication app)
        {
            BorrowReturnModel.Validator borrowReturnValidator = new BorrowReturnModel.Validator();

            // POST /borrow - Borrow a book
            app.MapPost("/borrow", (BorrowReturnModel request, LibraryService libraryService) =>
            {
                ValidationResult validationResult = borrowReturnValidator.Validate(request);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                if (!request.MemberId.HasValue)
                {
                    return Results.BadRequest(new { Message = "You must provide a Member ID to borrow a book." });
                }

                if (!libraryService.BookExists(request.BookId))
                {
                    return Results.NotFound(new { Message = "Book not found with the provided Book ID." });
                }

                if (!libraryService.MemberExists(request.MemberId.Value))
                {
                    return Results.NotFound(new { Message = "Member not found with the provided Member ID." });
                }

                try
                {
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
                    return Results.Problem("An error occurred while borrowing the book." + ex);
                }
            }).WithTags("Borrow");

            app.MapPost("/return", (int bookId, LibraryService libraryService) =>
                {
                    try
                    {
                        if (bookId <= 0)
                        {
                            return Results.BadRequest(new { Message = "Invalid Book ID." });
                        }

                        libraryService.ReturnBook(bookId);
                        return Results.Ok(new { Message = "Book returned successfully!" });
                    }
                    catch (InvalidOperationException ex)
                    {
                        return Results.Conflict(new { Error = ex.Message });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error returning book with ID {bookId}: {ex.Message}");
                        return Results.Problem("An error occurred while returning the book.");
                    }
                })
                .WithTags("Borrow");


            // GET /borrowed - Retrieve all borrowed books
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
            }).WithTags("Borrow");
        }
    }
}
