using Application;
using Domain;
using Domain.DTOs;
using LiMS.API.Models;
using FluentValidation.Results;

namespace LiMS.API.Routes
{
    public static class BooksRoutes
    {
        public static void MapBookRoutes(this WebApplication app)
        {
            BookModel.Validator bookValidator = new BookModel.Validator();

            // GET /books - Retrieve all books
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
            }).WithTags("Books");

            // POST /books - Create a new book
            app.MapPost("/books", (BookModel bookModel, LibraryService libraryService) =>
            {
                ValidationResult validationResult = bookValidator.Validate(bookModel);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                try
                {
                    if (bookModel == null)
                    {
                        return Results.BadRequest(new { Error = "Invalid request payload." });
                    }

                    Book book = new Book
                    {
                        Title = bookModel.Title,
                        Author = bookModel.Author,
                        IsBorrowed = bookModel.IsBorrowed,
                        BorrowedDate = bookModel.BorrowedDate,
                        BorrowedBy = bookModel.BorrowedBy
                    };

                    libraryService.AddBook(book);
                    // Return the created book with the assigned BookId
                    return Results.Created($"/books/{book.BookId}", new BookModel
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        Author = book.Author,
                        IsBorrowed = book.IsBorrowed,
                        BorrowedDate = book.BorrowedDate,
                        BorrowedBy = book.BorrowedBy
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding book: {ex.Message}");
                    return Results.Problem("An error occurred while adding the book.");
                }
            }).WithTags("Books");

            // PUT /books/{id} - Update an existing book
            app.MapPut("/books/{id}", (int id, BookModel bookModel, LibraryService libraryService) =>
            {
                ValidationResult validationResult = bookValidator.Validate(bookModel);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                try
                {
                    if (bookModel == null)
                    {
                        return Results.BadRequest(new { Error = "Invalid request payload." });
                    }

                    if (id != bookModel.BookId)
                    {
                        return Results.BadRequest(new { Error = "Book ID mismatch." });
                    }

                    Book? existingBook = libraryService.GetBookById(id);
                    if (existingBook == null)
                    {
                        return Results.NotFound(new { Error = "Book not found." });
                    }

                    existingBook.Title = bookModel.Title;
                    existingBook.Author = bookModel.Author;
                    existingBook.IsBorrowed = bookModel.IsBorrowed;
                    existingBook.BorrowedDate = bookModel.BorrowedDate;
                    existingBook.BorrowedBy = bookModel.BorrowedBy;

                    libraryService.UpdateBook(existingBook);
                    return Results.Ok(new { Message = "Book updated successfully!" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the book.");
                }
            }).WithTags("Books");

            // DELETE /books/{id} - Delete a book
            app.MapDelete("/books/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Book? book = libraryService.GetBookById(id);
                    if (book == null)
                    {
                        return Results.NotFound(new { Error = "Book not found." });
                    }

                    libraryService.DeleteBook(id);
                    return Results.Ok(new { Message = "Book deleted successfully!" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while deleting the book.");
                }
            }).WithTags("Books");

            // PATCH /books/{id} - Partially update a book
            app.MapPatch("/books/{id}", (int id, BookUpdateDto updateDto, LibraryService libraryService) =>
            {
                try
                {
                    if (updateDto == null)
                    {
                        return Results.BadRequest(new { Error = "Invalid request payload." });
                    }

                    Book? existingBook = libraryService.GetBookById(id);
                    if (existingBook == null)
                    {
                        return Results.NotFound(new { Error = "Book not found." });
                    }

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

                    libraryService.UpdateBook(existingBook);
                    return Results.Ok(new { Message = "Book updated successfully!" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating book with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the book.");
                }
            }).WithTags("Books");
        }
    }
}
