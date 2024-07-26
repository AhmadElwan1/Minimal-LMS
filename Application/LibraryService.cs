using LiMS.Domain;

namespace LiMS.Application
{
    public class LibraryService(IRepository<Book> bookRepository, IRepository<Member> memberRepository)
    {
        // Book operations

        public void AddBook(Book book)
        {
            bookRepository.Add(book);
        }

        public void UpdateBook(Book book)
        {
            bookRepository.Update(book);
        }

        public void DeleteBook(int bookID)
        {
            bookRepository.Delete(bookID);
        }

        public List<Book> GetAllBooks()
        {
            return bookRepository.GetAll();
        }

        public Book GetBookById(int bookID)
        {
            return bookRepository.GetById(bookID);
        }

        // Member operations

        public void AddMember(Member member)
        {
            memberRepository.Add(member);
        }

        public void UpdateMember(Member member)
        {
            memberRepository.Update(member);
        }

        public void DeleteMember(int memberID)
        {
            memberRepository.Delete(memberID);
        }

        public List<Member> GetAllMembers()
        {
            return memberRepository.GetAll();
        }

        public Member GetMemberById(int memberID)
        {
            return memberRepository.GetById(memberID);
        }

        public void BorrowBook(int bookID, int memberID)
        {
            Book book = bookRepository.GetById(bookID);
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

            Member member = memberRepository.GetById(memberID);
            if (member == null)
            {
                Console.WriteLine($"Member with ID {memberID} not found.");
                return;
            }

            book.IsBorrowed = true;
            book.BorrowedDate = DateTime.Now;
            book.BorrowedBy = memberID;

            bookRepository.Update(book);
            Console.WriteLine($"Book '{book.Title}' borrowed by {member.Name}.");
        }

        public void ReturnBook(int bookID)
        {
            Book book = bookRepository.GetById(bookID);
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

            bookRepository.Update(book);
            Console.WriteLine($"Book '{book.Title}' returned successfully.");
        }

        public List<Book> GetAllBorrowedBooks()
        {
            return bookRepository.GetAll().Where(b => b.IsBorrowed).ToList();
        }

    }
}