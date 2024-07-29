using System;
using Moq;
using Presentation;
using Application;
using Domain;
using Xunit;
using System.IO;

namespace LiMS.Tests.Presentation
{
    public class ProgramTests
    {
        private readonly Mock<IBookManagement> _mockBookManagement;
        private readonly Mock<IMemberManagement> _mockMemberManagement;
        private readonly Mock<IBorrowReturnBooks> _mockBorrowReturnBooks;
        private readonly Mock<LibraryService> _mockLibraryService;
        private readonly Program _program;

        public ProgramTests()
        {
            _mockBookManagement = new Mock<IBookManagement>();
            _mockMemberManagement = new Mock<IMemberManagement>();
            _mockBorrowReturnBooks = new Mock<IBorrowReturnBooks>();
            _mockLibraryService = new Mock<LibraryService>(
                Mock.Of<IRepository<Book>>(),
                Mock.Of<IRepository<Member>>()
            );

            _program = new Program(
                _mockBookManagement.Object,
                _mockMemberManagement.Object,
                _mockBorrowReturnBooks.Object,
                _mockLibraryService.Object
            );
        }

        [Fact]
        public void Run_ShouldCallManageBooksWhenOption1IsSelected()
        {
            // Arrange
            var input = "1\n6\n"; // Select "Manage Books" then "Exit"
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _program.Run();

            // Assert
            _mockBookManagement.Verify(b => b.ManageBooks(_mockLibraryService.Object), Times.Once);
        }

        [Fact]
        public void Run_ShouldCallManageMembersWhenOption2IsSelected()
        {
            // Arrange
            var input = "2\n6\n"; // Select "Manage Members" then "Exit"
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _program.Run();

            // Assert
            _mockMemberManagement.Verify(m => m.ManageMembers(_mockLibraryService.Object), Times.Once);
        }

        [Fact]
        public void Run_ShouldCallBorrowBookWhenOption3IsSelected()
        {
            // Arrange
            var input = "3\n6\n"; // Select "Borrow a Book" then "Exit"
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _program.Run();

            // Assert
            _mockBorrowReturnBooks.Verify(b => b.BorrowBook(_mockLibraryService.Object), Times.Once);
        }

        [Fact]
        public void Run_ShouldCallReturnBookWhenOption4IsSelected()
        {
            // Arrange
            var input = "4\n6\n"; // Select "Return a Book" then "Exit"
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _program.Run();

            // Assert
            _mockBorrowReturnBooks.Verify(b => b.ReturnBook(_mockLibraryService.Object), Times.Once);
        }

        [Fact]
        public void Run_ShouldCallViewAllBorrowedBooksWhenOption5IsSelected()
        {
            // Arrange
            var input = "5\n6\n"; // Select "View All Borrowed Books" then "Exit"
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _program.Run();

            // Assert
            _mockBorrowReturnBooks.Verify(b => b.ViewAllBorrowedBooks(_mockLibraryService.Object), Times.Once);
        }

        [Fact]
        public void Run_ShouldExitWhenOption6IsSelected()
        {
            // Arrange
            var input = "6\n"; // Select "Exit"
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act & Assert
            var exception = Record.Exception(() => _program.Run());
            Assert.Null(exception); // Ensure no exception is thrown
        }

        [Fact]
        public void Run_ShouldHandleInvalidInputGracefully()
        {
            // Arrange
            var input = "99\n1\n6\n"; // Select invalid option "99", then "Manage Books", then "Exit"
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Redirect Console.Out to a StringWriter to capture the output
            using (var stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);

                // Act
                _program.Run();

                // Assert
                var output = stringWriter.ToString();
                Assert.Contains("Invalid input. Please enter a number from 1 to 6.", output);
                Assert.Contains("Data saved. Goodbye!", output);
            }
        }
    }
}
