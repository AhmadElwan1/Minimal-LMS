using Application;
using Domain;
using Moq;

namespace LiMS.Tests.Application
{
    public class LibraryServiceTests
    {
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly Mock<IRepository<Member>> _mockMemberRepository;
        private readonly LibraryService _libraryService;

        public LibraryServiceTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockMemberRepository = new Mock<IRepository<Member>>();
            _libraryService = new LibraryService(_mockBookRepository.Object, _mockMemberRepository.Object);
        }

        [Fact]
        public void AddBook_ShouldCallAddOnBookRepository()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Test Book", Author = "Test Author" };

            // Act
            _libraryService.AddBook(book);

            // Assert
            _mockBookRepository.Verify(repo => repo.Add(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public void UpdateBook_ShouldCallUpdateOnBookRepository()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Updated Book", Author = "Updated Author" };

            // Act
            _libraryService.UpdateBook(book);

            // Assert
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public void DeleteBook_ShouldCallDeleteOnBookRepository()
        {
            // Act
            _libraryService.DeleteBook(1);

            // Assert
            _mockBookRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void GetAllBooks_ShouldReturnBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1" },
                new Book { BookId = 2, Title = "Book 2" }
            };
            _mockBookRepository.Setup(repo => repo.GetAll()).Returns(books);

            // Act
            var result = _libraryService.GetAllBooks();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Title == "Book 1");
            Assert.Contains(result, b => b.Title == "Book 2");
        }

        [Fact]
        public void GetBookById_ShouldReturnBook()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Test Book" };
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);

            // Act
            var result = _libraryService.GetBookById(1);

            // Assert
            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public void AddMember_ShouldCallAddOnMemberRepository()
        {
            // Arrange
            var member = new Member { MemberID = 1, Name = "Test Member" };

            // Act
            _libraryService.AddMember(member);

            // Assert
            _mockMemberRepository.Verify(repo => repo.Add(It.IsAny<Member>()), Times.Once);
        }

        [Fact]
        public void UpdateMember_ShouldCallUpdateOnMemberRepository()
        {
            // Arrange
            var member = new Member { MemberID = 1, Name = "Updated Member" };

            // Act
            _libraryService.UpdateMember(member);

            // Assert
            _mockMemberRepository.Verify(repo => repo.Update(It.IsAny<Member>()), Times.Once);
        }

        [Fact]
        public void DeleteMember_ShouldCallDeleteOnMemberRepository()
        {
            // Act
            _libraryService.DeleteMember(1);

            // Assert
            _mockMemberRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void GetAllMembers_ShouldReturnMembers()
        {
            // Arrange
            var members = new List<Member>
            {
                new Member { MemberID = 1, Name = "Member 1" },
                new Member { MemberID = 2, Name = "Member 2" }
            };
            _mockMemberRepository.Setup(repo => repo.GetAll()).Returns(members);

            // Act
            var result = _libraryService.GetAllMembers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Name == "Member 1");
            Assert.Contains(result, m => m.Name == "Member 2");
        }

        [Fact]
        public void GetMemberById_ShouldReturnMember()
        {
            // Arrange
            var member = new Member { MemberID = 1, Name = "Test Member" };
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            // Act
            var result = _libraryService.GetMemberById(1);

            // Assert
            Assert.Equal("Test Member", result.Name);
        }

        [Fact]
        public void BorrowBook_ShouldUpdateBookAndCallUpdateOnBookRepository()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = false };
            var member = new Member { MemberID = 1, Name = "Test Member" };

            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            // Act
            _libraryService.BorrowBook(1, 1);

            // Assert
            Assert.True(book.IsBorrowed);
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public void BorrowBook_ShouldNotBorrowIfAlreadyBorrowed()
        {
            // Arrange
            var book = new Book
            {
                BookId = 1,
                Title = "Test Book",
                IsBorrowed = true
            };
            var member = new Member
            {
                MemberID = 1,
                Name = "Test Member"
            };

            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _libraryService.BorrowBook(1, 1);

            // Assert
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Never);

            var output = consoleOutput.ToString().Trim();
            var expectedOutput = "This book is already borrowed.";
            Assert.Contains(expectedOutput, output);
        }

        [Fact]
        public void ReturnBook_ShouldUpdateBookAndCallUpdateOnBookRepository()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = true };

            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);

            // Act
            _libraryService.ReturnBook(1);

            // Assert
            Assert.False(book.IsBorrowed);
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public void ReturnBook_ShouldNotUpdateBookIfNotBorrowed()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = false };
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);

            // Act
            _libraryService.ReturnBook(1);

            // Assert
            // Verify that the Update method was not called because the book is not borrowed
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public void GetAllBorrowedBooks_ShouldReturnOnlyBorrowedBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", IsBorrowed = true },
                new Book { BookId = 2, Title = "Book 2", IsBorrowed = false }
            };
            _mockBookRepository.Setup(repo => repo.GetAll()).Returns(books);

            // Act
            var result = _libraryService.GetAllBorrowedBooks();

            // Assert
            Assert.Single(result);
            Assert.Contains(result, b => b.Title == "Book 1");
        }
    }
}
