using Application;
using Domain;
using Moq;
using Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace LiMS.Tests.Presentation
{
    public class BookManagementTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<IRepository<Book>> _mockBookRepo;
        private readonly Mock<IRepository<Member>> _mockMemberRepo;
        private readonly LibraryService _libraryService;
        private readonly BookManagement _bookManagement;

        public BookManagementTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mockBookRepo = new Mock<IRepository<Book>>();
            _mockMemberRepo = new Mock<IRepository<Member>>();

            // Initialize LibraryService with mocked repositories
            _libraryService = new LibraryService(_mockBookRepo.Object, _mockMemberRepo.Object);
            _bookManagement = new BookManagement();
        }

        [Fact]
        public void AddNewBook_ShouldAddBookSuccessfully()
        {
            // Arrange
            _mockBookRepo.Setup(r => r.GetAll()).Returns(new List<Book>());

            var input = "Title\nAuthor\n";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _bookManagement.AddNewBook(_libraryService);

            // Assert
            _mockBookRepo.Verify(r => r.Add(It.IsAny<Book>()), Times.Once);
        }

        [Theory]
        [InlineData("\nAuthor\nTitle\nAuthor\n", "Title cannot be empty. Please enter a valid title.")]
        [InlineData("Title\n\nTitle\nAuthor\n", "Author cannot be empty. Please enter a valid author.")]
        public void AddNewBook_ShouldHandleEmptyInputs(string input, string expectedMessage)
        {
            // Arrange
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            _mockBookRepo.Setup(r => r.GetAll()).Returns(new List<Book>());

            // Act
            _bookManagement.AddNewBook(_libraryService);

            // Assert
            _mockBookRepo.Verify(r => r.Add(It.IsAny<Book>()), Times.Once);

            string output = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, output);
            Assert.Contains("Book added successfully!", output);
        }

        [Fact]
        public void ManageBooks_ShouldPromptForNewBookDetails()
        {
            // Arrange
            var input = "1\nBook Title\nBook Author\n5\n"; // Simulate user input: choice to add a new book, title, author, and exit

            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            _mockBookRepo.Setup(r => r.GetAll()).Returns(new List<Book>());

            // Act
            _bookManagement.ManageBooks(_libraryService);

            // Assert
            string finalOutput = stringWriter.ToString().Trim();

            // Check that expected prompts and messages are in the output
            Assert.Contains("Enter details for the new book:", finalOutput);
            Assert.Contains("Title: ", finalOutput);
            Assert.Contains("Author: ", finalOutput);
            Assert.Contains("Book added successfully!", finalOutput);

            // Verify that AddBook was called with correct Book object
            _mockBookRepo.Verify(r => r.Add(It.Is<Book>(b =>
                b.Title == "Book Title" &&
                b.Author == "Book Author" &&
                b.IsBorrowed == false
            )), Times.Once);
        }

        [Fact]
        public void ManageBooks_ShouldPromptForBookUpdateId()
        {
            // Arrange
            var input = "1\nNew Title\nNew Author\n5\n";
            var expectedMessages = new[]
            {
                "Enter ID of the book to update:",
                "New title (leave blank to keep current):",
                "New author (leave blank to keep current):",
                "Book updated successfully!"
            };

            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var existingBook = new Book { BookId = 1, Title = "Original Title", Author = "Original Author", IsBorrowed = false };
            _mockBookRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns<int>(id => id == 1 ? existingBook : null);
            _mockBookRepo.Setup(r => r.Update(It.IsAny<Book>())).Verifiable();

            // Act
            _bookManagement.UpdateBook(_libraryService);

            // Assert
            string finalOutput = stringWriter.ToString().Trim();

            // Check that all expected messages are in the output
            foreach (var expectedMessage in expectedMessages)
            {
                Assert.Contains(expectedMessage, finalOutput);
            }
        }


        [Fact]
        public void ManageBooks_ShouldPromptForBookDeletionId()
        {
            // Arrange
            var input = "3\n5\n";
            var expectedMessage = "Enter ID of the book to delete:";

            using var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            using var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            _mockBookRepo.Setup(r => r.Delete(It.IsAny<int>())).Verifiable();

            // Act
            _bookManagement.DeleteBook(_libraryService);

            // Assert
            string finalOutput = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, finalOutput);

            _mockBookRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ManageBooks_ShouldDisplayAllBooks()
        {
            // Arrange
            var input = "4\n5\n";
            var expectedMessage = "===== All Books =====";

            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            _mockBookRepo.Setup(r => r.GetAll()).Returns(new List<Book>
            {
                new Book { BookId = 1, Title = "Test Book", Author = "Test Author", IsBorrowed = false }
            });

            // Act
            _bookManagement.ManageBooks(_libraryService);

            // Assert
            string finalOutput = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, finalOutput);
            Assert.Contains("Test Book", finalOutput);
            Assert.Contains("Test Author", finalOutput);

            _mockBookRepo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void ManageBooks_ShouldHandleInvalidInput()
        {
            // Arrange
            var input = "invalid\n5\n";
            var expectedMessage = "Invalid input. Please enter a number from 1 to 5.";

            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _bookManagement.ManageBooks(_libraryService);

            // Assert
            string finalOutput = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, finalOutput);

            _mockBookRepo.Verify(r => r.Add(It.IsAny<Book>()), Times.Never);
            _mockBookRepo.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
            _mockBookRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            _mockBookRepo.Verify(r => r.GetAll(), Times.Never);
        }

        [Theory]
        [InlineData("1\n", "Book deleted successfully!")]
        [InlineData("invalid\n", "Invalid input. Please enter a valid book ID.")]
        [InlineData("999\n", "Book deleted successfully!")]
        public void DeleteBook_ShouldHandleVariousInputs(string input, string expectedMessage)
        {
            // Arrange
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            if (input.Contains("999"))
            {
                _mockBookRepo.Setup(r => r.Delete(It.Is<int>(id => id == 999))).Verifiable();
            }
            else if (input.Contains("invalid"))
            {
                _mockBookRepo.Setup(r => r.Delete(It.IsAny<int>())).Throws<FormatException>();
            }
            else
            {
                _mockBookRepo.Setup(r => r.Delete(It.Is<int>(id => id == 1))).Verifiable();
            }

            // Act
            _bookManagement.DeleteBook(_libraryService);

            // Assert
            var finalOutput = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, finalOutput);

            if (expectedMessage == "Book deleted successfully!")
            {
                _mockBookRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Once);
            }
            else
            {
                _mockBookRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            }
        }
    }
}
