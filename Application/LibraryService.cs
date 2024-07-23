using LiMS.Domain;

namespace LiMS.Application
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

        public void AddBook(Book book)
        {
            _bookRepository.Add(book);
        }

        public void UpdateBook(Book book)
        {
            _bookRepository.Update(book);
        }

        public void DeleteBook(int bookID)
        {
            _bookRepository.Delete(bookID);
        }

        public List<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public Book GetBookById(int bookID)
        {
            return _bookRepository.GetById(bookID);
        }

        // Member operations

        public void AddMember(Member member)
        {
            _memberRepository.Add(member);
        }

        public void UpdateMember(Member member)
        {
            _memberRepository.Update(member);
        }

        public void DeleteMember(int memberID)
        {
            _memberRepository.Delete(memberID);
        }

        public List<Member> GetAllMembers()
        {
            return _memberRepository.GetAll();
        }

        public Member GetMemberById(int memberID)
        {
            return _memberRepository.GetById(memberID);
        }

        public void BorrowBook(int bookID, int memberID)
        {
            Book book = _bookRepository.GetById(bookID);
            if (book == null)
            {
                Console.WriteLine($"Book with ID {bookID} not found.");
                return;
            }

            if (book.IsBorrowed)
            {
                Console.WriteLine("This book is already borrowed.");
                return;
            }

            Member member = _memberRepository.GetById(memberID);
            if (member == null)
            {
                Console.WriteLine($"Member with ID {memberID} not found.");
                return;
            }

            book.IsBorrowed = true;
            book.BorrowedDate = DateTime.Now;
            book.BorrowedBy = memberID;

            _bookRepository.Update(book);
            Console.WriteLine($"Book '{book.Title}' borrowed by {member.Name}.");
        }

        public void ReturnBook(int bookID)
        {
            Book book = _bookRepository.GetById(bookID);
            if (book == null)
            {
                Console.WriteLine($"Book with ID {bookID} not found.");
                return;
            }

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

        public List<Book> GetAllBorrowedBooks()
        {
            return _bookRepository.GetAll().Where(b => b.IsBorrowed).ToList();
        }

    }
}