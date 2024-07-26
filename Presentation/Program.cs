using LiMS.Application;
using LiMS.Infrastructure;

namespace Presentation
{
    public class Program(LibraryService libraryService)
    {
        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine(@"
===== Library Management System =====
1. Manage Books
2. Manage Members
3. Borrow a Book
4. Return a Book
5. View All Borrowed Books
6. Exit
");
                Console.Write("Enter your choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        BookManagement.ManageBooks(libraryService);
                        break;
                    case "2":
                        MemberManagement.ManageMembers(libraryService);
                        break;
                    case "3":
                        BorrowReturnBooks.BorrowBook(libraryService);
                        break;
                    case "4":
                        BorrowReturnBooks.ReturnBook(libraryService);
                        break;
                    case "5":
                        BorrowReturnBooks.ViewAllBorrowedBooks(libraryService);
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a number from 1 to 6.");
                        break;
                }
            }

            Console.WriteLine("Data saved. Goodbye!");
        }

        public static void Main(string[] args)
        {
            // Example of using Program with a real LibraryService
            string bookFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Books.json";
            string memberFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Members.json";
            LibraryService libraryService = new LibraryService(new BookRepository(bookFile), new MemberRepository(memberFile));
            Program program = new Program(libraryService);
            program.Run();
        }
    }
}
