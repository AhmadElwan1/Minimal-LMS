using System;
using Xunit;
using Moq;
using LiMS.Application;
using LiMS.Domain;
using System.Collections.Generic;
using Presentation;

namespace LiMS.Tests.Presentation
{
    public class BorrowReturnBooksTests
    {
        private readonly Mock<IConsole> _mockConsole = new();

        [Fact]
        public void BorrowBook_ValidInput_ShouldBorrowBook()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var bookId = 1;
            var memberId = 1;

            mockLibraryService.Setup(s => s.BorrowBook(bookId, memberId));

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns(bookId.ToString())  // Book ID
                .Returns(memberId.ToString())  // Member ID
                .Returns("5"); // Exit after borrowing book
            Console.SetIn(_mockConsole.Object.In);

            // Act
            BorrowReturnBooks.BorrowBook(mockLibraryService.Object);

            // Assert
            mockLibraryService.Verify(s => s.BorrowBook(bookId, memberId), Times.Once);
        }

        [Fact]
        public void ReturnBook_ValidInput_ShouldReturnBook()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var bookId = 1;

            mockLibraryService.Setup(s => s.ReturnBook(bookId));

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns(bookId.ToString())  // Book ID
                .Returns("5"); // Exit after returning book
            Console.SetIn(_mockConsole.Object.In);

            // Act
            BorrowReturnBooks.ReturnBook(mockLibraryService.Object);

            // Assert
            mockLibraryService.Verify(s => s.ReturnBook(bookId), Times.Once);
        }

        [Fact]
        public void ViewAllBorrowedBooks_ShouldDisplayAllBorrowedBooks()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var borrowedBooks = new List<Book>
            {
                new() { BookId = 1, Title = "Book 1", IsBorrowed = true, BorrowedBy = 1, BorrowedDate = DateTime.Now },
                new() { BookId = 2, Title = "Book 2", IsBorrowed = true, BorrowedBy = 2, BorrowedDate = DateTime.Now }
            };

            mockLibraryService.Setup(s => s.GetAllBooks()).Returns(borrowedBooks);

            _mockConsole.Setup(c => c.WriteLine(It.IsAny<string>()));

            Console.SetOut(_mockConsole.Object.Out);

            // Act
            BorrowReturnBooks.ViewAllBorrowedBooks(mockLibraryService.Object);

            // Assert
            _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Exactly(borrowedBooks.Count));
        }
    }
}
