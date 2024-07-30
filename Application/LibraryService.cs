using Domain;

namespace Application
{
    public class LibraryService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Member> _memberRepository;

        public LibraryService(IRepository<Book> bookRepository, IRepository<Member> memberRepository)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
        }

        // Book operations

        public virtual void AddBook(Book book)
        {
            _bookRepository.Add(book);
        }

        public virtual void UpdateBook(Book book)
        {
            _bookRepository.Update(book);
        }

        public virtual void DeleteBook(int bookID)
        {
            _bookRepository.Delete(bookID);
        }

        public virtual List<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public virtual Book GetBookById(int bookID)
        {
            return _bookRepository.GetById(bookID);
        }

        // Member operations

        public virtual void AddMember(Member member)
        {
            _memberRepository.Add(member);
        }

        public virtual void UpdateMember(Member member)
        {
            _memberRepository.Update(member);
        }

        public virtual void DeleteMember(int memberID)
        {
            _memberRepository.Delete(memberID);
        }

        public virtual List<Member> GetAllMembers()
        {
            return _memberRepository.GetAll();
        }

        public virtual Member GetMemberById(int memberID)
        {
            return _memberRepository.GetById(memberID);
        }

        public virtual void BorrowBook(int bookID, int memberID)
        {
            Book book = _bookRepository.GetById(bookID);

            if (book.IsBorrowed)
            {
                Console.WriteLine("This book is already borrowed.");
                return;
            }

            Member member = _memberRepository.GetById(memberID);

            book.IsBorrowed = true;
            book.BorrowedDate = DateTime.Now;
            book.BorrowedBy = memberID;

            _bookRepository.Update(book);
            Console.WriteLine($"Book '{book.Title}' borrowed by {member.Name}.");
        }

        public virtual void ReturnBook(int bookID)
        {
            Book book = _bookRepository.GetById(bookID);

            if (!book.IsBorrowed)
            {
                Console.WriteLine("This book is not currently borrowed.");
                return;
            }

            book.IsBorrowed = false;
            book.BorrowedDate = null;
            book.BorrowedBy = null;

            _bookRepository.Update(book);
            Console.WriteLine($"Book '{book.Title}' returned successfully.");
        }

        public virtual List<Book> GetAllBorrowedBooks()
        {
            return _bookRepository.GetAll().Where(b => b.IsBorrowed).ToList();
        }
    }
}