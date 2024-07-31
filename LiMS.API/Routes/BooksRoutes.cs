using Application;
using Domain;
using Domain.DTOs;
using FluentValidation;
using FluentValidation.Results;
using LiMS.API.Models;

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
                    List<BookModel> bookModels = books.Select(b => new BookModel
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        Author = b.Author,
                        IsBorrowed = b.IsBorrowed,
                        BorrowedDate = b.BorrowedDate,
                        BorrowedBy = b.BorrowedBy
                    }).ToList();

                    return Results.Ok(bookModels);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving books: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving books.");
                }
            })
            .WithTags("Books");

            app.MapPost("/books", (BookModel bookModel, LibraryService libraryService, IValidator<BookModel> validator) =>
            {
                ValidationResult validationResult = validator.Validate(bookModel);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    Book book = new Book
                    {
                        BookId = bookModel.BookId,
                        Title = bookModel.Title,
                        Author = bookModel.Author,
                        IsBorrowed = bookModel.IsBorrowed,
                        BorrowedDate = bookModel.BorrowedDate,
                        BorrowedBy = bookModel.BorrowedBy
                    };
                    libraryService.AddBook(book);
                    return Results.Created($"/books/{book.BookId}", bookModel);
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

            app.MapPut("/books/{id}", (int id, BookModel bookModel, LibraryService libraryService, IValidator<BookModel> validator) =>
            {
                ValidationResult validationResult = validator.Validate(bookModel);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    if (id != bookModel.BookId)
                        return Results.BadRequest(new { Error = "Book ID mismatch." });

                    Book? existingBook = libraryService.GetBookById(id);
                    if (existingBook == null)
                        return Results.NotFound(new { Error = "Book not found." });

                    Book updatedBook = new Book
                    {
                        BookId = bookModel.BookId,
                        Title = bookModel.Title,
                        Author = bookModel.Author,
                        IsBorrowed = bookModel.IsBorrowed,
                        BorrowedDate = bookModel.BorrowedDate,
                        BorrowedBy = bookModel.BorrowedBy
                    };

                    libraryService.UpdateBook(updatedBook);
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
                    Book? book = libraryService.GetBookById(id);
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

            app.MapPatch("/books/{id}", (int id, BookUpdateDto updateDto, LibraryService libraryService, IValidator<BookModel> validator) =>
            {
                if (updateDto == null)
                    return Results.BadRequest(new { Error = "Invalid update data." });

                try
                {
                    Book? existingBook = libraryService.GetBookById(id);
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

                    ValidationResult validationResult = validator.Validate(new BookModel
                    {
                        BookId = existingBook.BookId,
                        Title = existingBook.Title,
                        Author = existingBook.Author,
                        IsBorrowed = existingBook.IsBorrowed,
                        BorrowedDate = existingBook.BorrowedDate,
                        BorrowedBy = existingBook.BorrowedBy
                    });

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
