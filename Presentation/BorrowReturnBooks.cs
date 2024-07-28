using Application;
using Domain;

namespace Presentation
{
    public interface IBorrowReturnBooks
    {
        void BorrowBook(LibraryService libraryService);
        void ReturnBook(LibraryService libraryService);
        void ViewAllBorrowedBooks(LibraryService libraryService);
    }

    public class BorrowReturnBooks : IBorrowReturnBooks
    {
        public void BorrowBook(LibraryService libraryService)
        {
            Console.Write("\nEnter ID of the book to borrow: ");
            if (int.TryParse(Console.ReadLine(), out int bookID))
            {
                Console.Write("Enter your member ID: ");
                if (int.TryParse(Console.ReadLine(), out int memberID))
                {
                    libraryService.BorrowBook(bookID, memberID);
                }
                else
                {
                    Console.WriteLine("Invalid member ID. Borrowing failed.");
                }
            }
            else
            {
                Console.WriteLine("Invalid book ID. Borrowing failed.");
            }
        }

        public void ReturnBook(LibraryService libraryService)
        {
            Console.Write("\nEnter ID of the book to return: ");
            if (int.TryParse(Console.ReadLine(), out int bookID))
            {
                libraryService.ReturnBook(bookID);
            }
            else
            {
                Console.WriteLine("Invalid book ID. Returning failed.");
            }
        }

        public void ViewAllBorrowedBooks(LibraryService libraryService)
        {
            Console.WriteLine("\n===== All Borrowed Books =====");
            foreach (Book book in libraryService.GetAllBooks().FindAll(b => b.IsBorrowed))
            {
                Console.WriteLine($"Book ID: {book.BookId}, Title: {book.Title}, " +
                                  $"Borrowed by Member ID: {book.BorrowedBy}, Due Date: {book.BorrowedDate}");
            }
        }
    }
}
