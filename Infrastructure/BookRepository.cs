using Newtonsoft.Json;
using LiMS.Domain;

namespace LiMS.Infrastructure
{
    public class BookRepository(string booksFile) : IRepository<Book>
    {
        public List<Book> GetAll()
        {
            if (!File.Exists(booksFile))
                return [];

            string booksJson = File.ReadAllText(booksFile);
            return JsonConvert.DeserializeObject<List<Book>>(booksJson);
        }

        public Book GetById(int id)
        {
            return GetAll().Find(b => b.BookId == id);
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
            int index = books.FindIndex(b => b.BookId == entity.BookId);
            if (index != -1)
            {
                books[index] = entity;
                SaveChanges(books);
            }
        }

        public void Delete(int id)
        {
            List<Book> books = GetAll();
            books.RemoveAll(b => b.BookId == id);
            SaveChanges(books);
        }

        private void SaveChanges(List<Book> books)
        {
            string booksJson = JsonConvert.SerializeObject(books, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(booksFile, booksJson);
        }
    }

}

