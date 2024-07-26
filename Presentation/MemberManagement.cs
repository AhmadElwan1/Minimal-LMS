using LiMS.Application;
using LiMS.Domain;

namespace Presentation
{
    public static class MemberManagement
    {
        public static void ManageMembers(LibraryService libraryService)
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
                        AddNewMember(libraryService);
                        break;
                    case "2":
                        UpdateMember(libraryService);
                        break;
                    case "3":
                        DeleteMember(libraryService);
                        break;
                    case "4":
                        ViewAllMembers(libraryService);
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

        public static void AddNewMember(LibraryService libraryService)
        {
            Console.WriteLine("\nEnter details for the new member:");

            string name;
            do
            {
                Console.Write("Name: ");
                name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name cannot be empty. Please enter a valid name.");
                }

            } while (string.IsNullOrWhiteSpace(name));

            string email = "";
            bool emailAlreadyExists = false;

            do
            {
                Console.Write("Email: ");
                email = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Email cannot be empty. Please enter a valid email.");
                    continue;
                }

                emailAlreadyExists = libraryService.GetAllMembers().Any(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (emailAlreadyExists)
                {
                    Console.WriteLine($"Email '{email}' is already in use. Please enter a different email.");
                }

            } while (string.IsNullOrWhiteSpace(email) || emailAlreadyExists);

            Member newMember = new Member
            {
                MemberID = libraryService.GetAllMembers().Count + 1,
                Name = name,
                Email = email
            };

            libraryService.AddMember(newMember);
            Console.WriteLine("Member added successfully!");
        }
        public static void UpdateMember(LibraryService libraryService)
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
        public static void DeleteMember(LibraryService libraryService)
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
        public static void ViewAllMembers(LibraryService libraryService)
        {
            Console.WriteLine("\n===== All Members =====");
            foreach (Member member in libraryService.GetAllMembers())
            {
                Console.WriteLine($"ID: {member.MemberID}, Name: {member.Name}, Email: {member.Email}");
            }
        }
    }
}
