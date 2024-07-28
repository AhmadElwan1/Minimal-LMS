using Application;
using Domain;
using Moq;
using Presentation;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace LiMS.Tests.Presentation
{
    public class BookManagementTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<LibraryService> _mockLibraryService;
        private readonly BookManagement _bookManagement;

        public BookManagementTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mockLibraryService = new Mock<LibraryService>(
                Mock.Of<IRepository<Book>>(),
                Mock.Of<IRepository<Member>>()
            );
            _bookManagement = new BookManagement();
        }

        [Fact]
        public void AddNewBook_ShouldAddBookSuccessfully()
        {
            // Arrange
            _mockLibraryService.Setup(l => l.GetAllBooks()).Returns(new List<Book>());

            var input = "Title\nAuthor\n";
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _bookManagement.AddNewBook(_mockLibraryService.Object);

            // Assert
            _mockLibraryService.Verify(l => l.AddBook(It.IsAny<Book>()), Times.Once);
        }

        [Theory]
        [InlineData("\nAuthor\nTitle\nAuthor\n", "Title cannot be empty. Please enter a valid title.")]
        [InlineData("Title\n\nTitle\nAuthor\n", "Author cannot be empty. Please enter a valid author.")]
        public void AddNewBook_ShouldHandleEmptyInputs(string input, string expectedMessage)
        {
            // Arrange
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            _mockLibraryService.Setup(l => l.GetAllBooks()).Returns(new List<Book>());

            // Act
            _bookManagement.AddNewBook(_mockLibraryService.Object);

            // Assert
            _mockLibraryService.Verify(l => l.AddBook(It.IsAny<Book>()), Times.Once);

            string output = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, output);
            Assert.Contains("Book added successfully!", output);
        }

        [Theory]
        [InlineData("1\n5\n", "Enter details for the new book:")]
        [InlineData("2\n5\n", "Enter ID of the book to update:")]
        [InlineData("3\n5\n", "Enter ID of the book to delete:")]
        [InlineData("4\n5\n", "===== All Books =====")]
        [InlineData("invalid\n5\n", "Invalid input. Please enter a number from 1 to 5.")]
        public void ManageBooks_ShouldInvokeExpectedMethods(string input, string expectedMessage)
        {
            // Arrange
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Setup mocks
            _mockLibraryService.ResetCalls(); // Reset method invocation counts

            if (expectedMessage == "===== All Books =====")
            {
                // Setup mock to return a list of books
                _mockLibraryService.Setup(l => l.GetAllBooks()).Returns(new List<Book>
                {
                    new Book { BookId = 1, Title = "Test Book", Author = "Test Author", IsBorrowed = false }
                });
            }

            // Act
            try
            {
                _bookManagement.ManageBooks(_mockLibraryService.Object);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                _testOutputHelper.WriteLine($"Exception during test execution: {ex.Message}");
                throw; // Re-throw to make sure the test fails and displays the issue
            }

            // Assert
            string finalOutput = stringWriter.ToString().Trim();

            // Validate output based on expected message
            if (expectedMessage == "Invalid input. Please enter a number from 1 to 5.")
            {
                // Ensure no book management method was called for invalid input
                _mockLibraryService.Verify(l => l.AddBook(It.IsAny<Book>()), Times.Never);
                _mockLibraryService.Verify(l => l.UpdateBook(It.IsAny<Book>()), Times.Never);
                _mockLibraryService.Verify(l => l.DeleteBook(It.IsAny<int>()), Times.Never);
                _mockLibraryService.Verify(l => l.GetAllBooks(), Times.Never);
            }
            else
            {
                // For valid inputs, ensure the respective methods are called
                if (expectedMessage == "Enter details for the new book:")
                {
                    Assert.Contains("Enter details for the new book:", finalOutput);
                }
                else if (expectedMessage == "Enter ID of the book to update:")
                {
                    _mockLibraryService.Verify(l => l.UpdateBook(It.IsAny<Book>()), Times.Once);
                }
                else if (expectedMessage == "Enter ID of the book to delete:")
                {
                    _mockLibraryService.Verify(l => l.DeleteBook(It.IsAny<int>()), Times.Once);
                }
                else if (expectedMessage == "===== All Books =====")
                {
                    _mockLibraryService.Verify(l => l.GetAllBooks(), Times.Once);

                    // Verify that the output contains the book details
                    Assert.Contains("===== All Books =====", finalOutput);
                    Assert.Contains("Test Book", finalOutput);
                    Assert.Contains("Test Author", finalOutput);
                }
            }

            // Ensure the output contains the expected message
            Assert.Contains(expectedMessage, finalOutput);
        }

        [Theory]
        [InlineData("1\nNew Title\nNew Author\n", "Book updated successfully!", "New Title", "New Author")]
        [InlineData("1\nNew Title\n\n", "Book updated successfully!", "New Title", "Original Author")]
        [InlineData("1\n\nNew Author\n", "Book updated successfully!", "Original Title", "New Author")]
        [InlineData("1\n\n\n", "Book updated successfully!", "Original Title", "Original Author")]
        [InlineData("invalid\nNew Title\nNew Author\n", "Invalid input. Please enter a valid book ID.", "", "")]
        [InlineData("999\nNew Title\nNew Author\n", "Book not found.", "", "")]
        public void UpdateBook_ShouldHandleVariousInputs(string input, string expectedMessage, string expectedTitle, string expectedAuthor)
        {
            // Arrange
            var bookId = 1;
            var originalBook = new Book { BookId = bookId, Title = "Original Title", Author = "Original Author", IsBorrowed = false };
            var updatedBook = new Book { BookId = bookId, Title = expectedTitle, Author = expectedAuthor, IsBorrowed = false };

            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Setup mocks
            _mockLibraryService.ResetCalls(); // Reset method invocation counts

            if (input.Contains("999"))
            {
                // Handle non-existent book scenario
                _mockLibraryService.Setup(l => l.GetBookById(It.Is<int>(id => id == 999))).Returns((Book)null);
            }
            else if (input.Contains("invalid"))
            {
                // Handle invalid ID scenario
                _mockLibraryService.Setup(l => l.GetBookById(It.IsAny<int>())).Throws<FormatException>();
            }
            else
            {
                _mockLibraryService.Setup(l => l.GetBookById(bookId)).Returns(originalBook);
            }

            // Act
            _bookManagement.UpdateBook(_mockLibraryService.Object);

            // Assert
            var finalOutput = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, finalOutput);

            if (expectedMessage == "Book updated successfully!")
            {
                _mockLibraryService.Verify(l => l.UpdateBook(It.Is<Book>(b =>
                    b.BookId == bookId &&
                    b.Title == expectedTitle &&
                    b.Author == expectedAuthor
                )), Times.Once);
            }
            else
            {
                _mockLibraryService.Verify(l => l.UpdateBook(It.IsAny<Book>()), Times.Never);
            }
        }

        [Theory]
        [InlineData("1\n", "Book deleted successfully!")]
        [InlineData("invalid\n", "Invalid input. Please enter a valid book ID.")]
        [InlineData("999\n", "Book deleted successfully!")]
        public void DeleteBook_ShouldHandleVariousInputs(string input, string expectedMessage)
        {
            // Arrange
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Setup mocks
            _mockLibraryService.ResetCalls(); // Reset method invocation counts

            if (input.Contains("999"))
            {
                // Setup mock to handle a valid but non-existent book ID
                _mockLibraryService.Setup(l => l.DeleteBook(It.Is<int>(id => id == 999))).Verifiable();
            }
            else if (input.Contains("invalid"))
            {
                // Setup mock to throw exception on invalid input
                _mockLibraryService.Setup(l => l.DeleteBook(It.IsAny<int>())).Throws<FormatException>();
            }
            else
            {
                // Setup mock to handle valid book ID
                _mockLibraryService.Setup(l => l.DeleteBook(It.Is<int>(id => id == 1))).Verifiable();
            }

            // Act
            _bookManagement.DeleteBook(_mockLibraryService.Object);

            // Assert
            var finalOutput = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, finalOutput);

            if (expectedMessage == "Book deleted successfully!")
            {
                _mockLibraryService.Verify(l => l.DeleteBook(It.IsAny<int>()), Times.Once);
            }
            else
            {
                _mockLibraryService.Verify(l => l.DeleteBook(It.IsAny<int>()), Times.Never);
            }
        }

    }
}
