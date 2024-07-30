using Application;
using Domain;

namespace LiMS.API.Routes
{
    public static class BorrowRoutes
    {
        public static void MapBorrowRoutes(this WebApplication app)
        {
            app.MapPost("/api/borrow", (BorrowRequest request, LibraryService libraryService) =>
            {
                try
                {
                    if (request == null || request.BookId <= 0 || request.MemberId <= 0)
                        return Results.BadRequest("Invalid request data.");

                    libraryService.BorrowBook(request.BookId, request.MemberId);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing borrow request: {ex.Message}");
                    return Results.Problem("An error occurred while processing the borrow request.");
                }
            });

            app.MapPost("/api/return", (int bookId, LibraryService libraryService) =>
            {
                try
                {
                    if (bookId <= 0)
                        return Results.BadRequest("Invalid book ID.");

                    libraryService.ReturnBook(bookId);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing return request for book ID {bookId}: {ex.Message}");
                    return Results.Problem("An error occurred while processing the return request.");
                }
            });

            app.MapGet("/api/borrowed", (LibraryService libraryService) =>
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
            });
        }
    }
}