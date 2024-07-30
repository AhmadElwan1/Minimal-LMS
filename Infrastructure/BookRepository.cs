using Domain;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class BookRepository : IRepository<Book>
    {
        private readonly string _booksFile;

        public BookRepository(string booksFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Books.json")
        {
            _booksFile = booksFile;
        }

        public List<Book> GetAll()
        {
            if (!File.Exists(_booksFile))
                return new List<Book>();

            try
            {
                string booksJson = File.ReadAllText(_booksFile);
                List<Book> books = JsonConvert.DeserializeObject<List<Book>>(booksJson) ?? new List<Book>();
                return books;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error reading file {_booksFile}: {ex.Message}");
                return new List<Book>();
            }
        }

        public Book GetById(int id)
        {
            List<Book> books = GetAll();
            return books.FirstOrDefault(b => b.BookId == id);
        }

        public void Add(Book entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Book title cannot be empty.");

            if (string.IsNullOrWhiteSpace(entity.Author))
                throw new ArgumentException("Book author cannot be empty.");

            // Check for duplicate ID
            List<Book> books = GetAll();
            if (books.Any(b => b.BookId == entity.BookId))
                throw new InvalidOperationException($"A book with ID {entity.BookId} already exists.");

            books.Add(entity);
            SaveChanges(books);
        }

        public void Update(Book entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Book title cannot be empty.");

            if (string.IsNullOrWhiteSpace(entity.Author))
                throw new ArgumentException("Book author cannot be empty.");

            List<Book> books = GetAll();
            int index = books.FindIndex(b => b.BookId == entity.BookId);
            if (index == -1)
                throw new KeyNotFoundException($"No book found with ID {entity.BookId}.");

            books[index] = entity;
            SaveChanges(books);
        }

        public void Delete(int id)
        {
            List<Book> books = GetAll();
            if (!books.Any(b => b.BookId == id))
                throw new KeyNotFoundException($"No book found with ID {id}.");

            books.RemoveAll(b => b.BookId == id);
            SaveChanges(books);
        }

        private void SaveChanges(List<Book> books)
        {
            try
            {
                string booksJson = JsonConvert.SerializeObject(books, Formatting.Indented);
                File.WriteAllText(_booksFile, booksJson);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error writing file {_booksFile}: {ex.Message}");
            }
        }
    }
}