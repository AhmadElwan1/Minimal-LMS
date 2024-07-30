using Domain;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class MemberRepository : IRepository<Member>
    {
        private readonly string _membersFile;

        public MemberRepository(string membersFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\Infrastructure\\Members.json")
        {
            _membersFile = membersFile;
        }

        public List<Member> GetAll()
        {
            if (!File.Exists(_membersFile))
                return new List<Member>();

            try
            {
                string membersJson = File.ReadAllText(_membersFile);
                return JsonConvert.DeserializeObject<List<Member>>(membersJson) ?? new List<Member>();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error reading file {_membersFile}: {ex.Message}");
                return new List<Member>();
            }
        }

        public Member GetById(int id)
        {
            List<Member> members = GetAll();
            return members.FirstOrDefault(m => m.MemberID == id); // Return null if not found
        }

        public void Add(Member entity)
        {
            if (entity == null)
                throw new ArgumentException("Member data cannot be null.");

            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Member name cannot be empty.");

            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException("Member email cannot be empty.");

            List<Member> members = GetAll();
            if (members.Any(m => m.MemberID == entity.MemberID))
                throw new InvalidOperationException($"A member with ID {entity.MemberID} already exists.");

            if (members.Any(m => string.Equals(m.Email, entity.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"A member with email '{entity.Email}' already exists.");

            members.Add(entity);
            SaveChanges(members);
        }

        public void Update(Member entity)
        {
            if (entity == null)
                throw new ArgumentException("Member data cannot be null.");

            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Member name cannot be empty.");

            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException("Member email cannot be empty.");

            List<Member> members = GetAll();
            int index = members.FindIndex(m => m.MemberID == entity.MemberID);
            if (index == -1)
                throw new InvalidOperationException($"No member found with ID {entity.MemberID}.");

            if (members.Any(m => m.MemberID != entity.MemberID && string.Equals(m.Email, entity.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"A member with email '{entity.Email}' already exists.");

            members[index] = entity;
            SaveChanges(members);
        }

        public void Delete(int id)
        {
            List<Member> members = GetAll();
            if (!members.Any(m => m.MemberID == id))
                throw new InvalidOperationException($"No member found with ID {id}.");

            members.RemoveAll(m => m.MemberID == id);
            SaveChanges(members);
        }

        private void SaveChanges(List<Member> members)
        {
            try
            {
                string membersJson = JsonConvert.SerializeObject(members, Formatting.Indented);
                File.WriteAllText(_membersFile, membersJson);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error writing file {_membersFile}: {ex.Message}");
                throw;
            }
        }
    }
}