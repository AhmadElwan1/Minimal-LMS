using Application;
using Domain;

namespace LiMS.API.Routes
{
    public static class BooksRoutes
    {
        public static void MapBookRoutes(this WebApplication app)
        {
            app.MapGet("/api/books", (LibraryService libraryService) =>
            {
                try
                {
                    List<Book> books = libraryService.GetAllBooks();
                    return Results.Ok(books);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving books: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving books.");
                }
            });

            app.MapGet("/api/books/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Book book = libraryService.GetBookById(id);
                    return book != null ? Results.Ok(book) : Results.NotFound("Book not found.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving the book.");
                }
            });

            app.MapPost("/api/books", (Book book, LibraryService libraryService) =>
            {
                try
                {
                    if (book == null)
                        return Results.BadRequest("Invalid book data.");

                    libraryService.AddBook(book);
                    return Results.Created($"/api/books/{book.BookId}", book);
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
                    Console.WriteLine($"Error adding book: {ex.Message}");
                    return Results.Problem("An error occurred while adding the book.");
                }
            });

            app.MapPut("/api/books/{id}", (int id, Book book, LibraryService libraryService) =>
            {
                try
                {
                    if (book == null || id != book.BookId)
                        return Results.BadRequest("Book ID mismatch.");

                    Book existingBook = libraryService.GetBookById(id);
                    if (existingBook == null)
                        return Results.NotFound("Book not found.");

                    libraryService.UpdateBook(book);
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
                    Console.WriteLine($"Error updating book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the book.");
                }
            });

            app.MapDelete("/api/books/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Book book = libraryService.GetBookById(id);
                    if (book == null)
                        return Results.NotFound("Book not found.");

                    libraryService.DeleteBook(id);
                    return Results.Ok("Book deleted successfully!");
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while deleting the book.");
                }
            });
        }
    }
}
