using System;
using LiMS.Domain;
using LiMS.Infrastructure;
using LiMS.Application;


namespace LiMS
{
    public class Program
    {

        private static LibraryService libraryService;



        public static void Main(string[] args)
        {

            Initialize();


            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n===== Library Management System =====");
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
                        ManageBooks();
                        break;
                    case "2":
                        ManageMembers();
                        break;
                    case "3":
                        BorrowBook();
                        break;
                    case "4":
                        ReturnBook();
                        break;
                    case "5":
                        ViewAllBorrowedBooks();
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

        private static void Initialize()
        {
            string booksFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\LiMS.Infrastructure\\Books.json";
            string membersFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\LiMS.Infrastructure\\Members.json";

            var bookRepository = new BookRepository(booksFile);
            var memberRepository = new MemberRepository(membersFile);

            libraryService = new LibraryService(bookRepository, memberRepository);
        }

        private static void ManageBooks()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n===== Manage Books =====");
                Console.WriteLine("1. Add a new book");
                Console.WriteLine("2. Update a book");
                Console.WriteLine("3. Delete a book");
                Console.WriteLine("4. View all books");
                Console.WriteLine("5. Back to main menu");
                Console.Write("Enter your choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        AddNewBook();
                        break;
                    case "2":
                        UpdateBook();
                        break;
                    case "3":
                        DeleteBook();
                        break;
                    case "4":
                        ViewAllBooks();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a number from 1 to 5.");
                        break;
                }
            }
        }

        private static void AddNewBook()
        {
            Console.WriteLine("\nEnter details for the new book:");
            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Author: ");
            string author = Console.ReadLine();

            Book newBook = new Book
            {
                BookID = libraryService.GetAllBooks().Count + 1,
                Title = title,
                Author = author,
                IsBorrowed = false
            };

            libraryService.AddBook(newBook);
            Console.WriteLine("Book added successfully!");
        }

        private static void UpdateBook()
        {
            Console.Write("\nEnter ID of the book to update: ");
            if (int.TryParse(Console.ReadLine(), out int bookID))
            {
                Book bookToUpdate = libraryService.GetBookById(bookID);
                if (bookToUpdate != null)
                {
                    Console.Write("New title (leave blank to keep current): ");
                    string newTitle = Console.ReadLine();
                    Console.Write("New author (leave blank to keep current): ");
                    string newAuthor = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(newTitle))
                        bookToUpdate.Title = newTitle;
                    if (!string.IsNullOrWhiteSpace(newAuthor))
                        bookToUpdate.Author = newAuthor;

                    libraryService.UpdateBook(bookToUpdate);
                    Console.WriteLine("Book updated successfully!");
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid book ID.");
            }
        }

        private static void DeleteBook()
        {
            Console.Write("\nEnter ID of the book to delete: ");
            if (int.TryParse(Console.ReadLine(), out int bookID))
            {
                libraryService.DeleteBook(bookID);
                Console.WriteLine("Book deleted successfully!");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid book ID.");
            }
        }

        private static void ViewAllBooks()
        {
            Console.WriteLine("\n===== All Books =====");
            foreach (var book in libraryService.GetAllBooks())
            {
                Console.WriteLine($"ID: {book.BookID}, Title: {book.Title}, Author: {book.Author}, " +
                                  $"Borrowed: {book.IsBorrowed}, Borrowed Date: {book.BorrowedDate}");
            }
        }



        private static void ManageMembers()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n===== Manage Members =====");
                Console.WriteLine("1. Add a new member");
                Console.WriteLine("2. Update a member");
                Console.WriteLine("3. Delete a member");
                Console.WriteLine("4. View all members");
                Console.WriteLine("5. Back to main menu");
                Console.Write("Enter your choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        AddNewMember();
                        break;
                    case "2":
                        UpdateMember();
                        break;
                    case "3":
                        DeleteMember();
                        break;
                    case "4":
                        ViewAllMembers();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a number from 1 to 5.");
                        break;
                }
            }
        }

        private static void AddNewMember()
        {
            Console.WriteLine("\nEnter details for the new member:");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();

            Member newMember = new Member
            {
                MemberID = libraryService.GetAllMembers().Count + 1,
                Name = name,
                Email = email
            };

            libraryService.AddMember(newMember);
            Console.WriteLine("Member added successfully!");
        }

        private static void UpdateMember()
        {
            Console.Write("\nEnter ID of the member to update: ");
            if (int.TryParse(Console.ReadLine(), out int memberID))
            {
                Member memberToUpdate = libraryService.GetMemberById(memberID);
                if (memberToUpdate != null)
                {
                    Console.Write("New name (leave blank to keep current): ");
                    string newName = Console.ReadLine();
                    Console.Write("New email (leave blank to keep current): ");
                    string newEmail = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(newName))
                        memberToUpdate.Name = newName;
                    if (!string.IsNullOrWhiteSpace(newEmail))
                        memberToUpdate.Email = newEmail;

                    libraryService.UpdateMember(memberToUpdate);
                    Console.WriteLine("Member updated successfully!");
                }
                else
                {
                    Console.WriteLine("Member not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid member ID.");
            }
        }

        private static void DeleteMember()
        {
            Console.Write("\nEnter ID of the member to delete: ");
            if (int.TryParse(Console.ReadLine(), out int memberID))
            {
                libraryService.DeleteMember(memberID);
                Console.WriteLine("Member deleted successfully!");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid member ID.");
            }
        }

        private static void ViewAllMembers()
        {
            Console.WriteLine("\n===== All Members =====");
            foreach (var member in libraryService.GetAllMembers())
            {
                Console.WriteLine($"ID: {member.MemberID}, Name: {member.Name}, Email: {member.Email}");
            }
        }

        private static void BorrowBook()
        {
            Console.Write("\nEnter ID of the book to borrow: ");
            if (int.TryParse(Console.ReadLine(), out int bookID))
            {
                Console.Write("Enter your member ID: ");
                if (int.TryParse(Console.ReadLine(), out int memberID))
                {
                    libraryService.BorrowBook(bookID, memberID);
                    Console.WriteLine("Book borrowed successfully!");
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

        private static void ReturnBook()
        {
            Console.Write("\nEnter ID of the book to return: ");
            if (int.TryParse(Console.ReadLine(), out int bookID))
            {
                libraryService.ReturnBook(bookID);
                Console.WriteLine("Book returned successfully!");
            }
            else
            {
                Console.WriteLine("Invalid book ID. Returning failed.");
            }
        }

        private static void ViewAllBorrowedBooks()
        {
            Console.WriteLine("\n===== All Borrowed Books =====");
            var borrowedBooks = libraryService.GetAllBooks().FindAll(b => b.IsBorrowed);
            foreach (var book in borrowedBooks)
            {
                Console.WriteLine($"Book ID: {book.BookID}, Title: {book.Title}, " +
                                  $"Borrowed by Member ID: {book.BorrowedBy}, Due Date: {book.BorrowedDate}");
            }
        }
    }
}
