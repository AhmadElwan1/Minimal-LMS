/*using Application;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace LiMS.Tests.Application
{
    public class LibraryServiceTests
    {
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly Mock<IRepository<Member>> _mockMemberRepository;
        private readonly Mock<IValidator<Book>> _mockBookValidator;
        private readonly Mock<IValidator<Member>> _mockMemberValidator;
        private readonly Mock<IValidator<BorrowReturnDto>> _mockBorrowRequestValidator;
        private readonly LibraryService _libraryService;

        public LibraryServiceTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockMemberRepository = new Mock<IRepository<Member>>();
            _mockBookValidator = new Mock<IValidator<Book>>();
            _mockMemberValidator = new Mock<IValidator<Member>>();
            _mockBorrowRequestValidator = new Mock<IValidator<BorrowReturnDto>>(); 


            _libraryService = new LibraryService(
                _mockBookRepository.Object,
                _mockMemberRepository.Object,
                _mockBookValidator.Object,
                _mockMemberValidator.Object,
                _mockBorrowRequestValidator.Object
            );
        }

        [Fact]
        public void AddBook_ShouldCallAddOnBookRepository()
        {
            // Arrange
            Book book = new Book { BookId = 1, Title = "Test Book", Author = "Test Author" };
            _mockBookValidator.Setup(v => v.Validate(It.IsAny<Book>())).Returns(new ValidationResult());

            // Act
            _libraryService.AddBook(book);

            // Assert
            _mockBookRepository.Verify(repo => repo.Add(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public void UpdateBook_ShouldCallUpdateOnBookRepository()
        {
            // Arrange
            Book book = new Book { BookId = 1, Title = "Updated Book", Author = "Updated Author" };
            _mockBookValidator.Setup(v => v.Validate(It.IsAny<Book>())).Returns(new ValidationResult());

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
            List<Book> books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1" },
                new Book { BookId = 2, Title = "Book 2" }
            };
            _mockBookRepository.Setup(repo => repo.GetAll()).Returns(books);

            // Act
            List<Book> result = _libraryService.GetAllBooks();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Title == "Book 1");
            Assert.Contains(result, b => b.Title == "Book 2");
        }

        [Fact]
        public void GetBookById_ShouldReturnBook()
        {
            // Arrange
            Book book = new Book { BookId = 1, Title = "Test Book" };
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);

            // Act
            Book result = _libraryService.GetBookById(1);

            // Assert
            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public void AddMember_ShouldCallAddOnMemberRepository()
        {
            // Arrange
            Member member = new Member { MemberID = 1, Name = "Test Member" };
            _mockMemberValidator.Setup(v => v.Validate(It.IsAny<Member>())).Returns(new ValidationResult());

            // Act
            _libraryService.AddMember(member);

            // Assert
            _mockMemberRepository.Verify(repo => repo.Add(It.IsAny<Member>()), Times.Once);
        }

        [Fact]
        public void UpdateMember_ShouldCallUpdateOnMemberRepository()
        {
            // Arrange
            Member member = new Member { MemberID = 1, Name = "Updated Member" };
            _mockMemberValidator.Setup(v => v.Validate(It.IsAny<Member>())).Returns(new ValidationResult());

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
            List<Member> members = new List<Member>
            {
                new Member { MemberID = 1, Name = "Member 1" },
                new Member { MemberID = 2, Name = "Member 2" }
            };
            _mockMemberRepository.Setup(repo => repo.GetAll()).Returns(members);

            // Act
            List<Member> result = _libraryService.GetAllMembers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Name == "Member 1");
            Assert.Contains(result, m => m.Name == "Member 2");
        }

        [Fact]
        public void GetMemberById_ShouldReturnMember()
        {
            // Arrange
            Member member = new Member { MemberID = 1, Name = "Test Member" };
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            // Act
            Member result = _libraryService.GetMemberById(1);

            // Assert
            Assert.Equal("Test Member", result.Name);
        }

        [Fact]
        public void BorrowBook_ShouldValidateBorrowRequestDtoAndCallUpdateOnBookRepository()
        {
            // Arrange
            BorrowReturnDto borrowRequestDto = new BorrowReturnDto { BookId = 1, MemberId = 1 }; // Changed to BorrowRequestDto
            Book book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = false };
            Member member = new Member { MemberID = 1, Name = "Test Member" };

            _mockBorrowRequestValidator.Setup(v => v.Validate(It.IsAny<BorrowReturnDto>())).Returns(new ValidationResult()); // Changed to BorrowRequestDto
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            // Act
            _libraryService.BorrowBook(borrowRequestDto); // Changed to BorrowRequestDto

            // Assert
            _mockBorrowRequestValidator.Verify(v => v.Validate(It.IsAny<BorrowReturnDto>()), Times.Once); // Changed to BorrowRequestDto
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public void BorrowBook_ShouldThrowValidationExceptionIfInvalid()
        {
            // Arrange
            BorrowReturnDto borrowRequestDto = new BorrowReturnDto { BookId = 1, MemberId = 1 }; // Changed to BorrowRequestDto
            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("BookId", "Invalid Book ID.") });
            _mockBorrowRequestValidator.Setup(v => v.Validate(It.IsAny<BorrowReturnDto>())).Returns(validationResult); // Changed to BorrowRequestDto

            // Act & Assert
            ValidationException exception = Assert.Throws<ValidationException>(() => _libraryService.BorrowBook(borrowRequestDto)); // Changed to BorrowRequestDto
            Assert.Contains("Invalid Book ID.", exception.Message);
        }

        [Fact]
        public void BorrowBook_ShouldNotBorrowIfAlreadyBorrowed()
        {
            // Arrange
            BorrowReturnDto borrowRequestDto = new BorrowReturnDto { BookId = 1, MemberId = 1 }; // Changed to BorrowRequestDto
            Book book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = true };
            Member member = new Member { MemberID = 1, Name = "Test Member" };

            _mockBorrowRequestValidator.Setup(v => v.Validate(It.IsAny<BorrowReturnDto>())).Returns(new ValidationResult()); // Changed to BorrowRequestDto
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            // Act
            _libraryService.BorrowBook(borrowRequestDto); // Changed to BorrowRequestDto

            // Assert
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public void ReturnBook_ShouldUpdateBookAndCallUpdateOnBookRepository()
        {
            // Arrange
            Book book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = true };
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
            Book book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = false };
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);

            // Act
            _libraryService.ReturnBook(1);

            // Assert
            _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public void GetAllBorrowedBooks_ShouldReturnOnlyBorrowedBooks()
        {
            // Arrange
            List<Book> books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", IsBorrowed = true },
                new Book { BookId = 2, Title = "Book 2", IsBorrowed = false }
            };
            _mockBookRepository.Setup(repo => repo.GetAll()).Returns(books);

            // Act
            List<Book> result = _libraryService.GetAllBorrowedBooks();

            // Assert
            Assert.Single(result);
            Assert.Contains(result, b => b.Title == "Book 1");
        }
    }
}
*/