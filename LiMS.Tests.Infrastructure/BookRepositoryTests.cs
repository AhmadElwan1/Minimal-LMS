using Newtonsoft.Json;
using Domain;
using Infrastructure;

namespace LiMS.Tests.Infrastructure
{
    public class BookRepositoryTests
    {
        private readonly string _booksFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\LiMS.Tests\\test_books.json";

        private readonly BookRepository _repository;

        public BookRepositoryTests()
        {
            _repository = new BookRepository(_booksFile);
        }

        [Fact]
        public void Add_ShouldAddBookToRepository()
        {
            Book book = new Book { BookId = 1, Title = "Test Book" };
            _repository.Add(book);
            List<Book>? books = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(_booksFile));
            if (books != null) Assert.Contains(books, b => b.BookId == book.BookId);
        }

        [Fact]
        public void GetAll_ShouldReturnBooks()
        {
            Book book = new Book { BookId = 5, Title = "Test", Author = "Test" };
            _repository.Add(book);
            List<Book> books = _repository.GetAll();
            Assert.NotEmpty(books);
        }

        [Fact]
        public void GetById_ShouldReturnBook()
        {
            Book book = _repository.GetById(1);
            Assert.NotNull(book);
        }

        [Fact]
        public void Delete_ShouldRemoveBook()
        {
            Book book = new Book { BookId = 3, Title = "Test", Author = "Test"};
            _repository.Add(book);
            _repository.Delete(3);
            List<Book> books = _repository.GetAll();
            Assert.DoesNotContain(books, b => b.BookId == 3);
        }

        [Fact]
        public void Update_ShouldUpdateExistingBook()
        {
            // Arrange
            Book initialBook = new Book { BookId = 1, Title = "Initial Title" };
            Book updatedBook = new Book { BookId = 1, Title = "Updated Title" };
            _repository.Add(initialBook);

            // Act
            _repository.Update(updatedBook);

            // Assert
            List<Book>? books = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(_booksFile));
            Book? book = books?.Find(b => b.BookId == updatedBook.BookId);
            Assert.NotNull(book);
            Assert.Equal("Updated Title", book.Title);
        }


    }
}