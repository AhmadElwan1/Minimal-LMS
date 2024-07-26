using System;
using Xunit;
using Moq;
using LiMS.Application;
using System.IO;
using Presentation;

namespace LiMS.Tests.Presentation
{
    public class ProgramTests
    {
        [Theory]
        [InlineData("1")] // Manage Books
        [InlineData("2")] // Manage Members
        [InlineData("3")] // Borrow a Book
        [InlineData("4")] // Return a Book
        [InlineData("5")] // View All Borrowed Books
        [InlineData("6")] // Exit
        public void Run_MenuOption_Selected(string userInput)
        {

            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            // Mock Console.ReadLine() to simulate user input
            var mockConsole = new Mock<IConsole>();
            mockConsole.Setup(c => c.ReadLine()).Returns(userInput);
            Console.SetIn(mockConsole.Object.In);

            // Act
            program.Run();

            // Assert
            // Verify that corresponding method in each class is called based on user input
            switch (userInput)
            {
                case "1":
                    mockLibraryService.Verify(s => s.GetAllBooks(), Times.Once);
                    break;
                case "2":
                    mockLibraryService.Verify(s => s.GetAllMembers(), Times.Once);
                    break;
                case "3":
                    mockLibraryService.Verify(s => s.BorrowBook(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                    break;
                case "4":
                    mockLibraryService.Verify(s => s.ReturnBook(It.IsAny<int>()), Times.Once);
                    break;
                case "5":
                    mockLibraryService.Verify(s => s.GetAllBooks(), Times.Once);
                    break;
                case "6":
                    // Ensure the loop ends and the program exits
                    break;
                default:
                    Assert.True(false, "Invalid user input for testing.");
                    break;
            }
        }

        [Fact]
        public void Run_InvalidInput_ShouldPrintErrorMessage()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var mockConsole = new Mock<IConsole>();
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("invalid")
                .Returns("6"); // Exit after invalid input
            Console.SetIn(mockConsole.Object.In);

            // Act
            program.Run();

            // Assert
            mockConsole.Verify(c => c.WriteLine("Invalid input. Please enter a number from 1 to 6."), Times.Once);
        }
    }

    // Mock IConsole interface for testing purposes
    public interface IConsole
    {
        string ReadLine();
        void WriteLine(string value);
        TextReader In { get; }
        TextWriter Out { get; }
        TextWriter Error { get; }
    }
}
