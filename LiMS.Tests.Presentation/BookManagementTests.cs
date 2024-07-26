using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using LiMS.Application;
using LiMS.Domain;
using Presentation;
using LiMS.Mocks;

namespace LiMS.Tests.Presentation
{
    public class BookManagementTests
    {
        private readonly MockRepository<Book> _mockBookRepository = new MockRepository<Book>();
        private readonly Mock<IConsole> _mockConsole = new();

        [Fact]
        public void AddNewBook_ValidInput_ShouldAddBook()
        {
            // Arrange
            var mockLibraryService = new LibraryService(_mockBookRepository, null); // Pass mock repository

            var newBook = new Book
            {
                BookId = 1,
                Title = "Test Book",
                Author = "Test Author",
                IsBorrowed = false
            };

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("Test Book")  // Title
                .Returns("Test Author")  // Author
                .Returns("5"); // Exit after adding book
            Console.SetIn(new System.IO.StringReader("Test Book\nTest Author\n5\n"));
            Console.SetOut(new System.IO.StringWriter());

            // Act
            BorrowReturnBooks.AddNewBook(mockLibraryService);

            // Assert
            Assert.Contains(newBook, _mockBookRepository.GetAll());
        }

        [Fact]
        public void UpdateBook_ValidInput_ShouldUpdateBook()
        {
            // Arrange
            var existingBook = new Book
            {
                BookId = 1,
                Title = "Existing Book",
                Author = "Existing Author",
                IsBorrowed = false
            };

            _mockBookRepository.Add(existingBook);

            var mockLibraryService = new LibraryService(_mockBookRepository, null); // Pass mock repository

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("")  // No change to title
                .Returns("New Author")  // New author
                .Returns("5"); // Exit after updating book
            Console.SetIn(new System.IO.StringReader("\nNew Author\n5\n"));
            Console.SetOut(new System.IO.StringWriter());

            // Act
            BorrowReturnBooks.UpdateBook(mockLibraryService);

            // Assert
            var updatedBook = _mockBookRepository.GetById(1);
            Assert.Equal("Existing Book", updatedBook.Title);  // Title should not change
            Assert.Equal("New Author", updatedBook.Author);  // Author should update
        }

        [Fact]
        public void DeleteBook_ValidInput_ShouldDeleteBook()
        {
            // Arrange
            var existingBook = new Book
            {
                BookId = 1,
                Title = "Existing Book",
                Author = "Existing Author",
                IsBorrowed = false
            };

            _mockBookRepository.Add(existingBook);

            var mockLibraryService = new LibraryService(_mockBookRepository, null); // Pass mock repository

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("1")  // Book ID to delete
                .Returns("5"); // Exit after deleting book
            Console.SetIn(new System.IO.StringReader("1\n5\n"));
            Console.SetOut(new System.IO.StringWriter());

            // Act
            BorrowReturnBooks.DeleteBook(mockLibraryService);

            // Assert
            Assert.DoesNotContain(existingBook, _mockBookRepository.GetAll());
        }

        [Fact]
        public void ViewAllBooks_ShouldDisplayAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1", IsBorrowed = false },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2", IsBorrowed = true, BorrowedDate = DateTime.Now }
            };

            foreach (var book in books)
            {
                _mockBookRepository.Add(book);
            }

            var mockLibraryService = new LibraryService(_mockBookRepository, null); // Pass mock repository

            _mockConsole.Setup(c => c.WriteLine(It.IsAny<string>()));
            Console.SetOut(new System.IO.StringWriter());

            // Act
            BorrowReturnBooks.ViewAllBooks(mockLibraryService);

            // Assert
            _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Exactly(books.Count));
        }
    }
}
