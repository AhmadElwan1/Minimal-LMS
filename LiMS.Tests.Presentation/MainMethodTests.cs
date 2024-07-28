using System;
using Moq;
using Presentation;
using Application;
using Domain;
using Xunit;
using System.IO;
using Infrastructure;
using Xunit.Abstractions;

namespace LiMS.Tests.Presentation
{
    public class MainMethodTests
    {
        [Fact]
        public void Main_ShouldCallRunApplication()
        {
            // Arrange
            var mockBookManagement = new Mock<IBookManagement>();
            var mockMemberManagement = new Mock<IMemberManagement>();
            var mockBorrowReturnBooks = new Mock<IBorrowReturnBooks>();
            var mockLibraryService = new Mock<LibraryService>(
                Mock.Of<IRepository<Book>>(),
                Mock.Of<IRepository<Member>>()
            );

            var program = new Program(
                mockBookManagement.Object,
                mockMemberManagement.Object,
                mockBorrowReturnBooks.Object,
                mockLibraryService.Object
            );

            // Act & Assert
            var exception = Record.Exception(() => Program.Main([]));
            Assert.Null(exception); // Ensure no exception is thrown
        }

        [Fact]
        public void RunApplication_ShouldInitializeAndRunProgram()
        {
            // Arrange
            var bookFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Books.json";
            var memberFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Members.json";

            var mockBookManagement = new Mock<IBookManagement>();
            var mockMemberManagement = new Mock<IMemberManagement>();
            var mockBorrowReturnBooks = new Mock<IBorrowReturnBooks>();

            // Create mocks for the repositories
            var mockBookRepository = new Mock<BookRepository>(bookFile) { CallBase = true };
            var mockMemberRepository = new Mock<MemberRepository>(memberFile) { CallBase = true };

            var libraryService = new LibraryService(mockBookRepository.Object, mockMemberRepository.Object);

            var program = new Program(
                mockBookManagement.Object,
                mockMemberManagement.Object,
                mockBorrowReturnBooks.Object,
                libraryService
            );

            // Act
            // Redirect console output to a StringWriter
            using (var stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);

                // Call RunApplication
                Program.RunApplication();

                // Optionally check the console output
                var output = stringWriter.ToString();
                Assert.Contains("Data saved. Goodbye!", output);
            }

            // Assert that RunApplication initializes and runs the program correctly
            mockBookManagement.Verify(b => b.ManageBooks(It.IsAny<LibraryService>()), Times.AtLeastOnce());
            mockMemberManagement.Verify(m => m.ManageMembers(It.IsAny<LibraryService>()), Times.AtLeastOnce());
            mockBorrowReturnBooks.Verify(b => b.BorrowBook(It.IsAny<LibraryService>()), Times.AtLeastOnce());
            mockBorrowReturnBooks.Verify(b => b.ReturnBook(It.IsAny<LibraryService>()), Times.AtLeastOnce());
            mockBorrowReturnBooks.Verify(b => b.ViewAllBorrowedBooks(It.IsAny<LibraryService>()), Times.AtLeastOnce());
        }
    }
}
