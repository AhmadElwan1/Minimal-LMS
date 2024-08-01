using Domain;

namespace Application
{
    public class LibraryService
    {
        private readonly IRepository<Book> bookRepository;
        private readonly IRepository<Member> memberRepository;

        public LibraryService(
            IRepository<Book> bookRepository,
            IRepository<Member> memberRepository)
        {
            this.bookRepository = bookRepository;
            this.memberRepository = memberRepository;
        }

        private void AddEntity<T>(IRepository<T> repository, T entity) => repository.Add(entity);
        private void UpdateEntity<T>(IRepository<T> repository, T entity) => repository.Update(entity);
        private void DeleteEntity<T>(IRepository<T> repository, int id) => repository.Delete(id);

        public void AddBook(Book book)
        {
            AddEntity(bookRepository, book);
        }

        public void UpdateBook(Book book)
        {
            UpdateEntity(bookRepository, book);
        }

        public void DeleteBook(int bookID) => DeleteEntity(bookRepository, bookID);
        public List<Book> GetAllBooks() => bookRepository.GetAll();
        public Book GetBookById(int bookID) => bookRepository.GetById(bookID);

        public void AddMember(Member member)
        {
            AddEntity(memberRepository, member);
        }

        public void UpdateMember(Member member)
        {
            UpdateEntity(memberRepository, member);
        }

        public bool BookExists(int bookId) => bookRepository.GetById(bookId) != null;
        public bool MemberExists(int memberId) => memberRepository.GetById(memberId) != null;

        public void DeleteMember(int memberID) => DeleteEntity(memberRepository, memberID);
        public List<Member> GetAllMembers() => memberRepository.GetAll();
        public Member GetMemberById(int memberID) => memberRepository.GetById(memberID);

        public void BorrowBook(int bookId, int memberId)
        {
            Book? book = bookRepository.GetById(bookId);
            Member? member = memberRepository.GetById(memberId);

            if (book == null)
                throw new ArgumentException($"No book found with ID {bookId}");

            if (member == null)
                throw new ArgumentException($"No member found with ID {memberId}");

            if (book.IsBorrowed)
                throw new InvalidOperationException("This book is already borrowed.");

            book.IsBorrowed = true;
            book.BorrowedDate = DateTime.UtcNow; // Set the time to UTC
            book.BorrowedBy = memberId;

            UpdateEntity(bookRepository, book);
        }

        public void ReturnBook(int bookId)
        {
            Book? book = bookRepository.GetById(bookId);

            if (book == null)
                throw new ArgumentException($"No book found with ID {bookId}");

            if (!book.IsBorrowed)
                throw new InvalidOperationException("This book is not currently borrowed.");

            book.IsBorrowed = false;
            book.BorrowedDate = null; // Clear the BorrowedDate
            book.BorrowedBy = null; // Clear the BorrowedBy reference

            UpdateEntity(bookRepository, book);
        }


        public List<Book> GetAllBorrowedBooks() => bookRepository.GetAll().Where(b => b.IsBorrowed).ToList();
    }
}
