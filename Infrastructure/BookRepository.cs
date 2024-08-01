using Domain;

namespace Infrastructure
{
    public class BookRepository(ApplicationDbContext context) : IRepository<Book>
    {
        public List<Book> GetAll()
        {
            return context.Books.ToList();
        }

        public Book GetById(int id)
        {
            return context.Books.Find(id);
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
            if (context.Books.Any(b => b.BookId == entity.BookId))
                throw new InvalidOperationException($"A book with ID {entity.BookId} already exists.");

            context.Books.Add(entity);
            context.SaveChanges();
        }

        public void Update(Book entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Book title cannot be empty.");

            if (string.IsNullOrWhiteSpace(entity.Author))
                throw new ArgumentException("Book author cannot be empty.");

            Book? existingBook = context.Books.Find(entity.BookId);
            if (existingBook == null)
                throw new KeyNotFoundException($"No book found with ID {entity.BookId}.");

            context.Entry(existingBook).CurrentValues.SetValues(entity);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            Book? book = context.Books.Find(id);
            if (book == null)
                throw new KeyNotFoundException($"No book found with ID {id}.");

            context.Books.Remove(book);
            context.SaveChanges();
        }
    }
}
