using Newtonsoft.Json;
using LiMS.Domain;

namespace LiMS.Infrastructure
{
    public class BookRepository : IRepository<Book>
    {

        private readonly string _booksFile;

        public BookRepository(string booksFile)
        {
            _booksFile = booksFile;
        }

        public List<Book> GetAll()
        {
            if (!File.Exists(_booksFile))
                return new List<Book>();

            string booksJson = File.ReadAllText(_booksFile);
            return JsonConvert.DeserializeObject<List<Book>>(booksJson);
        }

        public Book GetById(int id)
        {
            return GetAll().Find(b => b.BookID == id);
        }

        public void Add(Book entity)
        {
            List<Book> books = GetAll();
            books.Add(entity);
            SaveChanges(books);
        }

        public void Update(Book entity)
        {
            List<Book> books = GetAll();
            int index = books.FindIndex(b => b.BookID == entity.BookID);
            if (index != -1)
            {
                books[index] = entity;
                SaveChanges(books);
            }
        }

        public void Delete(int id)
        {
            List<Book> books = GetAll();
            books.RemoveAll(b => b.BookID == id);
            SaveChanges(books);
        }

        private void SaveChanges(List<Book> books)
        {
            string booksJson = JsonConvert.SerializeObject(books, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_booksFile, booksJson);
        }
    }

}

