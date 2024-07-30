using Application;
using Domain;
using Domain.DTOs;
using FluentValidation;

namespace LiMS.API.Routes
{
    public static class BooksRoutes
    {
        public static void MapBookRoutes(this WebApplication app)
        {
            app.MapGet("/books", (LibraryService libraryService) =>
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
            })
            .WithTags("Books");

            app.MapGet("/books/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Book book = libraryService.GetBookById(id);
                    return book != null ? Results.Ok(book) : Results.NotFound(new { Error = "Book not found." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving the book.");
                }
            })
            .WithTags("Books");

            app.MapPost("/books", async (Book book, LibraryService libraryService, IValidator<Book> validator) =>
            {
                var validationResult = await validator.ValidateAsync(book);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    libraryService.AddBook(book);
                    return Results.Created($"/books/{book.BookId}", book);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding book: {ex.Message}");
                    return Results.Problem("An error occurred while adding the book.");
                }
            })
            .WithTags("Books");

            app.MapPut("/books/{id}", async (int id, Book book, LibraryService libraryService, IValidator<Book> validator) =>
            {
                var validationResult = await validator.ValidateAsync(book);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    if (book == null || id != book.BookId)
                        return Results.BadRequest(new { Error = "Book ID mismatch." });

                    Book existingBook = libraryService.GetBookById(id);
                    if (existingBook == null)
                        return Results.NotFound(new { Error = "Book not found." });

                    libraryService.UpdateBook(book);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the book.");
                }
            })
            .WithTags("Books");

            app.MapDelete("/books/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Book book = libraryService.GetBookById(id);
                    if (book == null)
                        return Results.NotFound(new { Error = "Book not found." });

                    libraryService.DeleteBook(id);
                    return Results.Ok(new { Message = "Book deleted successfully!" });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while deleting the book.");
                }
            })
            .WithTags("Books");

            app.MapPatch("/books/{id}", async (int id, BookUpdateDto updateDto, LibraryService libraryService, IValidator<Book> validator) =>
            {
                if (updateDto == null)
                    return Results.BadRequest(new { Error = "Invalid update data." });

                try
                {
                    Book existingBook = libraryService.GetBookById(id);
                    if (existingBook == null)
                        return Results.NotFound(new { Error = "Book not found." });

                    if (updateDto.Title != null)
                        existingBook.Title = updateDto.Title;

                    if (updateDto.Author != null)
                        existingBook.Author = updateDto.Author;

                    if (updateDto.IsBorrowed.HasValue)
                        existingBook.IsBorrowed = updateDto.IsBorrowed.Value;

                    if (updateDto.BorrowedDate.HasValue)
                        existingBook.BorrowedDate = updateDto.BorrowedDate.Value;

                    if (updateDto.BorrowedBy.HasValue)
                        existingBook.BorrowedBy = updateDto.BorrowedBy.Value;

                    var validationResult = await validator.ValidateAsync(existingBook);
                    if (!validationResult.IsValid)
                        return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                    libraryService.UpdateBook(existingBook);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the book.");
                }
            })
            .WithTags("Books");
        }
    }
}
