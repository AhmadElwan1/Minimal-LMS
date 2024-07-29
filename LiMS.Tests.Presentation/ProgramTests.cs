using Application;
using Domain;
using Moq;
using Presentation;
using Xunit;

namespace LiMS.Tests.Presentation
{
    public class ProgramTests
    {
        private readonly Mock<IBookManagement> _mockBookManagement;
        private readonly Mock<IMemberManagement> _mockMemberManagement;
        private readonly Mock<IBorrowReturnBooks> _mockBorrowReturnBooks;
        private readonly Mock<LibraryService> _mockLibraryService;
        private readonly Mock<IConsoleService> _mockConsoleService;
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
            _mockConsoleService = new Mock<IConsoleService>();

            _program = new Program(
                _mockBookManagement.Object,
                _mockMemberManagement.Object,
                _mockBorrowReturnBooks.Object,
                _mockLibraryService.Object,
                _mockConsoleService.Object
            );
        }

        [Fact]
        public void Run_ShouldHandleMenuChoices()
        {
            // Arrange
            _mockConsoleService.SetupSequence(c => c.ReadLine())
                .Returns("1")
                .Returns("2")
                .Returns("3")
                .Returns("4")
                .Returns("5")
                .Returns("6");

            // Act
            _program.Run();

            // Assert
            _mockBookManagement.Verify(b => b.ManageBooks(_mockLibraryService.Object), Times.Once);
            _mockMemberManagement.Verify(m => m.ManageMembers(_mockLibraryService.Object), Times.Once);
            _mockBorrowReturnBooks.Verify(b => b.BorrowBook(_mockLibraryService.Object), Times.Once);
            _mockBorrowReturnBooks.Verify(b => b.ReturnBook(_mockLibraryService.Object), Times.Once);
            _mockBorrowReturnBooks.Verify(b => b.ViewAllBorrowedBooks(_mockLibraryService.Object), Times.Once);
            _mockConsoleService.Verify(c => c.WriteLine("Data saved. Goodbye!"), Times.Once);
        }

        [Fact]
        public void Run_ShouldHandleInvalidInput()
        {
            // Arrange
            _mockConsoleService.SetupSequence(c => c.ReadLine())
                .Returns("invalid")
                .Returns("6");

            // Act
            _program.Run();

            // Assert
            _mockConsoleService.Verify(c => c.WriteLine("Invalid input. Please enter a number from 1 to 6."), Times.Once);
            _mockConsoleService.Verify(c => c.WriteLine("Data saved. Goodbye!"), Times.Once);
        }
    }
}
