using Application;
using Infrastructure;

namespace Presentation
{
    public class Program
    {
        private readonly IBookManagement _bookManagement;
        private readonly IMemberManagement _memberManagement;
        private readonly IBorrowReturnBooks _borrowReturnBooks;
        private readonly LibraryService _libraryService;

        public Program(
            IBookManagement bookManagement,
            IMemberManagement memberManagement,
            IBorrowReturnBooks borrowReturnBooks,
            LibraryService libraryService)
        {
            _bookManagement = bookManagement;
            _memberManagement = memberManagement;
            _borrowReturnBooks = borrowReturnBooks;
            _libraryService = libraryService;
        }

        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n===== Main Menu =====");
                Console.WriteLine("1. Manage Books");
                Console.WriteLine("2. Manage Members");
                Console.WriteLine("3. Borrow a Book");
                Console.WriteLine("4. Return a Book");
                Console.WriteLine("5. View All Borrowed Books");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        _bookManagement.ManageBooks(_libraryService);
                        break;
                    case "2":
                        _memberManagement.ManageMembers(_libraryService);
                        break;
                    case "3":
                        _borrowReturnBooks.BorrowBook(_libraryService);
                        break;
                    case "4":
                        _borrowReturnBooks.ReturnBook(_libraryService);
                        break;
                    case "5":
                        _borrowReturnBooks.ViewAllBorrowedBooks(_libraryService);
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
            RunApplication();
        }

        public static void RunApplication()
        {
            // Setup file paths
            string bookFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Books.json";
            string memberFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Members.json";

            // Create repositories
            var bookRepository = new BookRepository(bookFile);
            var memberRepository = new MemberRepository(memberFile);

            // Create service
            var libraryService = new LibraryService(bookRepository, memberRepository);

            // Create management instances
            IBookManagement bookManagement = new BookManagement();
            IMemberManagement memberManagement = new MemberManagement();
            IBorrowReturnBooks borrowReturnBooks = new BorrowReturnBooks();

            // Create and run the program
            var program = new Program(bookManagement, memberManagement, borrowReturnBooks, libraryService);
            program.Run();
        }
    }
}
