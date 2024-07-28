using Application;
using Domain;
using Moq;
using Presentation;
using System;
using System.Collections.Generic;
using Xunit;

namespace LiMS.Tests.Presentation
{

    public class BorrowReturnBooksTests
    {
        private readonly Mock<LibraryService> _mockLibraryService;
        private readonly BorrowReturnBooks _borrowReturnBooks;

        public BorrowReturnBooksTests()
        {
            _mockLibraryService = new Mock<LibraryService>(
                Mock.Of<IRepository<Book>>(),
                Mock.Of<IRepository<Member>>()
            );
            _borrowReturnBooks = new BorrowReturnBooks();
        }

        [Fact]
        public void BorrowBook_ShouldBorrowBookSuccessfully()
        {
            // Arrange
            var input = "1\n1\n";
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _borrowReturnBooks.BorrowBook(_mockLibraryService.Object);

            // Assert
            _mockLibraryService.Verify(l => l.BorrowBook(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ReturnBook_ShouldReturnBookSuccessfully()
        {
            // Arrange
            int bookID = 1;
            var input = $"{bookID}\n"; // Mock user input for book ID
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _borrowReturnBooks.ReturnBook(_mockLibraryService.Object);

            // Assert
            _mockLibraryService.Verify(l => l.ReturnBook(bookID), Times.Once);
        }

        [Fact]
        public void ReturnBook_ShouldHandleInvalidBookId()
        {
            // Arrange
            var input = "invalid\n"; // Mock user input for invalid book ID

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _borrowReturnBooks.ReturnBook(_mockLibraryService.Object);

            // Assert
            // Verify that ReturnBook method on LibraryService was not called
            _mockLibraryService.Verify(l => l.ReturnBook(It.IsAny<int>()), Times.Never);

            // Verify console output
            string output = stringWriter.ToString().Trim();
            Assert.Contains("Invalid book ID. Returning failed.", output);
        }


        [Fact]
        public void BorrowBook_ShouldHandleInvalidBookId()
        {
            // Arrange
            var input = "invalid\n1\n"; // Mock user input for invalid book ID

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _borrowReturnBooks.BorrowBook(_mockLibraryService.Object);

            // Assert
            // Verify that BorrowBook method on LibraryService was not called
            _mockLibraryService.Verify(l => l.BorrowBook(It.IsAny<int>(), It.IsAny<int>()), Times.Never);

            // Verify console output
            string output = stringWriter.ToString().Trim();
            Assert.Contains("Invalid book ID. Borrowing failed.", output);
        }

        [Fact]
        public void BorrowBook_ShouldHandleInvalidMemberId()
        {
            // Arrange
            var input = "1\ninvalid\n"; // Mock user input for invalid member ID

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _borrowReturnBooks.BorrowBook(_mockLibraryService.Object);

            // Assert
            // Verify that BorrowBook method on LibraryService was not called
            _mockLibraryService.Verify(l => l.BorrowBook(It.IsAny<int>(), It.IsAny<int>()), Times.Never);

            // Verify console output
            string output = stringWriter.ToString().Trim();
            Assert.Contains("Invalid member ID. Borrowing failed.", output);
        }

        [Fact]
        public void ViewAllBorrowedBooks_ShouldDisplayAllBorrowedBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", IsBorrowed = true, BorrowedBy = 1, BorrowedDate = DateTime.Now },
                new Book { BookId = 2, Title = "Book 2", IsBorrowed = false, BorrowedBy = 0, BorrowedDate = default },
                new Book { BookId = 3, Title = "Book 3", IsBorrowed = true, BorrowedBy = 2, BorrowedDate = DateTime.Now }
            };

            _mockLibraryService.Setup(l => l.GetAllBooks()).Returns(books);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _borrowReturnBooks.ViewAllBorrowedBooks(_mockLibraryService.Object);

            // Assert
            string output = stringWriter.ToString().Trim();
            Assert.Contains("===== All Borrowed Books =====", output);
            Assert.Contains("Book ID: 1, Title: Book 1, Borrowed by Member ID: 1", output);
            Assert.Contains("Book ID: 3, Title: Book 3, Borrowed by Member ID: 2", output);
            Assert.DoesNotContain("Book ID: 2, Title: Book 2", output); // Ensure non-borrowed books are not listed
        }

        // Test for ViewAllBorrowedBooks...
    }

}
