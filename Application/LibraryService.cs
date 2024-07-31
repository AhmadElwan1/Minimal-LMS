using Domain;

namespace Application
{
    public class LibraryService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Member> _memberRepository;

        public LibraryService(
            IRepository<Book> bookRepository,
            IRepository<Member> memberRepository)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
        }

        private void AddEntity<T>(IRepository<T> repository, T entity) => repository.Add(entity);
        private void UpdateEntity<T>(IRepository<T> repository, T entity) => repository.Update(entity);
        private void DeleteEntity<T>(IRepository<T> repository, int id) => repository.Delete(id);

        public void AddBook(Book book)
        {
            AddEntity(_bookRepository, book);
        }

        public void UpdateBook(Book book)
        {
            UpdateEntity(_bookRepository, book);
        }

        public void DeleteBook(int bookID) => DeleteEntity(_bookRepository, bookID);
        public List<Book> GetAllBooks() => _bookRepository.GetAll();
        public Book GetBookById(int bookID) => _bookRepository.GetById(bookID);

        public void AddMember(Member member)
        {
            AddEntity(_memberRepository, member);
        }

        public void UpdateMember(Member member)
        {
            UpdateEntity(_memberRepository, member);
        }

        public bool BookExists(int bookId) => _bookRepository.GetById(bookId) != null;
        public bool MemberExists(int memberId) => _memberRepository.GetById(memberId) != null;

        public void DeleteMember(int memberID) => DeleteEntity(_memberRepository, memberID);
        public List<Member> GetAllMembers() => _memberRepository.GetAll();
        public Member GetMemberById(int memberID) => _memberRepository.GetById(memberID);

        public void BorrowBook(int bookId, int memberId)
        {
            Book? book = _bookRepository.GetById(bookId);
            Member? member = _memberRepository.GetById(memberId);

            if (book == null)
                throw new ArgumentException($"No book found with ID {bookId}");

            if (member == null)
                throw new ArgumentException($"No member found with ID {memberId}");

            if (book.IsBorrowed)
                throw new InvalidOperationException("This book is already borrowed.");

            book.IsBorrowed = true;
            book.BorrowedDate = DateTime.Now;
            book.BorrowedBy = memberId;

            UpdateEntity(_bookRepository, book);
        }

        public void ReturnBook(int bookId)
        {
            Book? book = _bookRepository.GetById(bookId);

            if (book == null)
                throw new ArgumentException($"No book found with ID {bookId}");

            if (!book.IsBorrowed)
                throw new InvalidOperationException("This book is not currently borrowed.");

            book.IsBorrowed = false;
            book.BorrowedDate = null;
            book.BorrowedBy = null;

            UpdateEntity(_bookRepository, book);
        }

        public List<Book> GetAllBorrowedBooks() => _bookRepository.GetAll().Where(b => b.IsBorrowed).ToList();
    }
}
