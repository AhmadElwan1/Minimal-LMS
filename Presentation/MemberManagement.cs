using Application;
using Domain;

namespace Presentation
{
    public interface IMemberManagement
    {
        void ManageMembers(LibraryService libraryService);
        void AddNewMember(LibraryService libraryService);
        void UpdateMember(LibraryService libraryService);
        void DeleteMember(LibraryService libraryService);
        void ViewAllMembers(LibraryService libraryService);
    }

    public class MemberManagement : IMemberManagement
    {
        public void ManageMembers(LibraryService libraryService)
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

        public void AddNewMember(LibraryService libraryService)
        {
            Console.WriteLine("\nEnter details for the new member:");

            string name;
            do
            {
                Console.Write("Name: ");
                name = Console.ReadLine() ?? "";

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
                email = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Email cannot be empty. Please enter a valid email.");
                    continue;
                }

                emailAlreadyExists = libraryService.GetAllMembers().Any(m =>
                {
                    if (m.Email is null) return false;
                    return m.Email.Equals(email, StringComparison.OrdinalIgnoreCase);
                });

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

        public void UpdateMember(LibraryService libraryService)
        {
            Console.Write("\nEnter ID of the member to update: ");
            if (int.TryParse(Console.ReadLine(), out int memberID))
            {
                Member memberToUpdate = libraryService.GetMemberById(memberID);

                Console.Write("New name (leave blank to keep current): ");
                string newName = Console.ReadLine() ?? "";
                Console.Write("New email (leave blank to keep current): ");
                string newEmail = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(newName))
                    memberToUpdate.Name = newName;
                if (!string.IsNullOrWhiteSpace(newEmail))
                    memberToUpdate.Email = newEmail;

                libraryService.UpdateMember(memberToUpdate);
                Console.WriteLine("Member updated successfully!");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid member ID.");
            }
        }



        public void DeleteMember(LibraryService libraryService)
        {
            Console.Write("\nEnter ID of the member to delete: ");
            if (int.TryParse(Console.ReadLine(), out int memberID))
            {
                var member = libraryService.GetMemberById(memberID);
                if (member != null)
                {
                    libraryService.DeleteMember(memberID);
                    Console.WriteLine("Member deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Member not found. Please enter a valid member ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid member ID.");
            }
        }


        public void ViewAllMembers(LibraryService libraryService)
        {
            var members = libraryService.GetAllMembers();
            Console.WriteLine("\n===== All Members =====");
            if (members == null || !members.Any())
            {
                Console.WriteLine("No members found.");
                return;
            }

            foreach (var member in members)
            {
                Console.WriteLine($"ID: {member.MemberID}, Name: {member.Name}, Email: {member.Email}");
            }
        }


    }
}
