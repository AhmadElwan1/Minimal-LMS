using LiMS.Application;
using LiMS.Domain;

namespace Presentation
{
    public static class BookManagement
    {
        public static void ManageBooks(LibraryService libraryService)
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
                        AddNewBook(libraryService);
                        break;
                    case "2":
                        UpdateBook(libraryService);
                        break;
                    case "3":
                        DeleteBook(libraryService);
                        break;
                    case "4":
                        ViewAllBooks(libraryService);
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

        public static void AddNewBook(LibraryService libraryService)
        {
            Console.WriteLine("\nEnter details for the new book:");

            string title;
            do
            {
                Console.Write("Title: ");
                title = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(title))
                {
                    Console.WriteLine("Title cannot be empty. Please enter a valid title.");
                }

            } while (string.IsNullOrWhiteSpace(title));

            string author;
            do
            {
                Console.Write("Author: ");
                author = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(author))
                {
                    Console.WriteLine("Author cannot be empty. Please enter a valid author.");
                }

            } while (string.IsNullOrWhiteSpace(author));

            Book newBook = new Book
            {
                BookId = libraryService.GetAllBooks().Count + 1,
                Title = title,
                Author = author,
                IsBorrowed = false
            };

            libraryService.AddBook(newBook);
            Console.WriteLine("Book added successfully!");
        }

        public static void UpdateBook(LibraryService libraryService)
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

        public static void DeleteBook(LibraryService libraryService)
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

        public static void ViewAllBooks(LibraryService libraryService)
        {
            Console.WriteLine("\n===== All Books =====");
            foreach (Book book in libraryService.GetAllBooks())
            {
                Console.WriteLine($"ID: {book.BookId}, Title: {book.Title}, Author: {book.Author}, " +
                                  $"Borrowed: {book.IsBorrowed}, Borrowed Date: {book.BorrowedDate}");
            }
        }
    }
}
