using LiMS.Domain;
using LiMS.Infrastructure;
using Newtonsoft.Json;

namespace LiMS.Tests.Infrastructure
{
    public class BookRepositoryTests : IDisposable
    {
        private readonly string _tempFilePath;
        private readonly BookRepository _bookRepository;

        public BookRepositoryTests()
        {
            _tempFilePath = Path.GetTempFileName(); // Ensure this generates a valid temporary file path
            _bookRepository = new BookRepository(_tempFilePath); // Initialize _bookRepository
        }

        public void Dispose()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath); // Clean up temporary file after tests
            }
        }

        [Fact]
        public void GetAll_Should_Return_All_Books()
        {
            // Arrange
            List<Book> expectedBooks = new List<Book>
            {
                new Book { BookID = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookID = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookID = 3, Title = "Book 3", Author = "Author 3" }
            };

            SaveBooksToFile(expectedBooks);

            // Act
            List<Book> actualBooks = _bookRepository.GetAll();

            // Assert
            Assert.Equal(expectedBooks.Count, actualBooks.Count);
            foreach (var expectedBook in expectedBooks)
            {
                var actualBook = actualBooks.Single(b => b.BookID == expectedBook.BookID);
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
            List<Book> books = new List<Book>
            {
                new Book { BookID = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookID = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookID = 3, Title = "Book 3", Author = "Author 3" }
            };
            SaveBooksToFile(books);

            // Act
            Book foundBook = _bookRepository.GetById(2);

            // Assert
            Assert.NotNull(foundBook);
            Assert.Equal(2, foundBook.BookID);
            Assert.Equal("Book 2", foundBook.Title);
            Assert.Equal("Author 2", foundBook.Author);
        }

        [Fact]
        public void Add_Should_Add_New_Book()
        {
            // Arrange
            Book newBook = new Book { BookID = 4, Title = "New Book", Author = "New Author" };

            // Act
            _bookRepository.Add(newBook);

            // Assert
            List<Book> books = GetAllBooksFromFile();
            Book addedBook = books.SingleOrDefault(b => b.BookID == newBook.BookID);

            Assert.NotNull(addedBook);
            Assert.Equal(newBook.Title, addedBook.Title);
            Assert.Equal(newBook.Author, addedBook.Author);
        }

        [Fact]
        public void Update_Should_Update_Existing_Book()
        {
            // Arrange
            List<Book> books = new List<Book>
            {
                new Book { BookID = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookID = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookID = 3, Title = "Book 3", Author = "Author 3" }
            };
            SaveBooksToFile(books);

            Book updatedBook = new Book { BookID = 2, Title = "Updated Book", Author = "Updated Author" };

            // Act
            _bookRepository.Update(updatedBook);

            // Assert
            List<Book> updatedBooks = GetAllBooksFromFile();
            Book foundBook = updatedBooks.Single(b => b.BookID == updatedBook.BookID);
            Assert.Equal(updatedBook.Title, foundBook.Title);
            Assert.Equal(updatedBook.Author, foundBook.Author);
        }

        [Fact]
        public void Delete_Should_Delete_Existing_Book()
        {
            // Arrange
            List<Book> books = new List<Book>
            {
                new Book { BookID = 1, Title = "Book 1", Author = "Author 1" },
                new Book { BookID = 2, Title = "Book 2", Author = "Author 2" },
                new Book { BookID = 3, Title = "Book 3", Author = "Author 3" }
            };
            SaveBooksToFile(books);

            // Act
            _bookRepository.Delete(2);

            // Assert
            List<Book> remainingBooks = GetAllBooksFromFile();
            Assert.Equal(2, remainingBooks.Count);
            Assert.DoesNotContain(books.Single(b => b.BookID == 2), remainingBooks);
        }

        private void SaveBooksToFile(List<Book> books)
        {
            string booksJson = JsonConvert.SerializeObject(books);
            File.WriteAllText(_tempFilePath, booksJson);
        }

        private List<Book> GetAllBooksFromFile()
        {
            if (!File.Exists(_tempFilePath))
                return new List<Book>();

            string booksJson = File.ReadAllText(_tempFilePath);
            return JsonConvert.DeserializeObject<List<Book>>(booksJson);
        }
    }
}
