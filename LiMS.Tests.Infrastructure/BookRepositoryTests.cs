using LiMS.Domain;
using LiMS.Infrastructure;
using Newtonsoft.Json;

namespace LiMS.Tests.Infrastructure
{
    public class BookRepositoryTests : IDisposable
    {
        private readonly string _tempFilePath = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\LiMS.Tests.Infrastructure\\test_books.json";
        private readonly BookRepository _bookRepository;

        public BookRepositoryTests()
        {
            _bookRepository = new BookRepository(_tempFilePath); 
        }

        public void Dispose()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath); 
            }
        }

        [Fact]
        public void GetAll_Should_Return_All_Books()
        {
            // Arrange
            List<Book> expectedBooks =
            [
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookId = 3, Title = "Book 3", Author = "Author 3" }
            ];

            SaveBooksToFile(expectedBooks);

            // Act
            List<Book> actualBooks = _bookRepository.GetAll();

            // Assert
            Assert.Equal(expectedBooks.Count, actualBooks.Count);
            foreach (var expectedBook in expectedBooks)
            {
                var actualBook = actualBooks.Single(b => b.BookId == expectedBook.BookId);
                Assert.Equal(expectedBook.Title, actualBook.Title);
                Assert.Equal(expectedBook.Author, actualBook.Author);
            }
        }

        [Fact]
        public void GetAll_Should_Return_Empty_List_When_File_Not_Found()
        {
            // Arrange
            File.Delete(_tempFilePath); 

            // Act
            List<Book> actualBooks = _bookRepository.GetAll();

            // Assert
            Assert.Empty(actualBooks);
        }

        [Fact]
        public void GetById_Should_Return_Correct_Book()
        {
            // Arrange
            List<Book> books =
            [
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookId = 3, Title = "Book 3", Author = "Author 3" }
            ];
            SaveBooksToFile(books);

            // Act
            Book foundBook = _bookRepository.GetById(2);

            // Assert
            Assert.NotNull(foundBook);
            Assert.Equal(2, foundBook.BookId);
            Assert.Equal("Book 2", foundBook.Title);
            Assert.Equal("Author 2", foundBook.Author);
        }

        [Fact]
        public void Add_Should_Add_New_Book()
        {
            // Arrange
            Book newBook = new Book { BookId = 4, Title = "New Book 4", Author = "New Author 4" };

            // Act
            _bookRepository.Add(newBook);

            // Assert
            List<Book> booksAfterAdd = GetAllBooksFromFile();

            // Verify book is added correctly
            Assert.NotNull(booksAfterAdd);

            bool bookFound = false;
            foreach (var book in booksAfterAdd)
            {
                if (book.BookId == newBook.BookId &&
                    book.Title == newBook.Title &&
                    book.Author == newBook.Author)
                {
                    bookFound = true;
                }
            }

            Assert.True(bookFound, $"Failed to find {newBook.Title} by {newBook.Author} in the collection.");
        }


        [Fact]
        public void Update_Should_Update_Existing_Book()
        {
            // Arrange
            List<Book> books =
            [
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookId = 3, Title = "Book 3", Author = "Author 3" }
            ];
            SaveBooksToFile(books);

            Book updatedBook = new Book { BookId = 2, Title = "Updated Book", Author = "Updated Author" };

            // Act
            _bookRepository.Update(updatedBook);

            // Assert
            List<Book> updatedBooks = GetAllBooksFromFile();
            Book foundBook = updatedBooks.Single(b => b.BookId == updatedBook.BookId);
            Assert.Equal(updatedBook.Title, foundBook.Title);
            Assert.Equal(updatedBook.Author, foundBook.Author);
        }

        [Fact]
        public void Delete_Should_Delete_Existing_Book()
        {
            // Arrange
            List<Book> books =
            [
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookId = 3, Title = "Book 3", Author = "Author 3" }
            ];
            SaveBooksToFile(books);

            // Act
            _bookRepository.Delete(2);

            // Assert
            List<Book> remainingBooks = GetAllBooksFromFile();
            Assert.Equal(2, remainingBooks.Count);
            Assert.DoesNotContain(books.Single(b => b.BookId == 2), remainingBooks);
        }

        private void SaveBooksToFile(List<Book> books)
        {
            string booksJson = JsonConvert.SerializeObject(books);
            File.WriteAllText(_tempFilePath, booksJson);
        }

        private List<Book> GetAllBooksFromFile()
        {

            string booksJson = File.ReadAllText(_tempFilePath);
            return JsonConvert.DeserializeObject<List<Book>>(booksJson);
        }
    }
}
