using LiMS.Application;
using LiMS.Domain;

namespace Presentation
{
    public static class BorrowReturnBooks
    {
        public static void BorrowBook(LibraryService libraryService)
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

        public static void ReturnBook(LibraryService libraryService)
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

        public static void ViewAllBorrowedBooks(LibraryService libraryService)
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