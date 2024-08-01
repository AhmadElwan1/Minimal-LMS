using Domain;

namespace Infrastructure
{
    public class MemberRepository(ApplicationDbContext context) : IRepository<Member>
    {
        public List<Member> GetAll()
        {
            return context.Members.ToList();
        }

        public Member GetById(int id)
        {
            return context.Members.Find(id);
        }

        public void Add(Member entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Member name cannot be empty.");

            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException("Member email cannot be empty.");

            string emailToCheck = entity.Email.ToLower();

            if (context.Members.Any(m => m.MemberID == entity.MemberID))
                throw new InvalidOperationException($"A member with ID {entity.MemberID} already exists.");

            if (context.Members.Any(m => m.Email.ToLower() == emailToCheck))
                throw new InvalidOperationException($"A member with email '{entity.Email}' already exists.");

            context.Members.Add(entity);
            context.SaveChanges();
        }

        public void Update(Member entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Member name cannot be empty.");

            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException("Member email cannot be empty.");

            Member? existingMember = context.Members.Find(entity.MemberID);
            if (existingMember == null)
                throw new KeyNotFoundException($"No member found with ID {entity.MemberID}.");

            // Ensure the email is in lowercase for a case-insensitive comparison
            string emailToCheck = entity.Email.ToLower();

            // Check if a member with the same email already exists
            if (context.Members.Any(m => m.MemberID != entity.MemberID && m.Email.ToLower() == emailToCheck))
                throw new InvalidOperationException($"A member with email '{entity.Email}' already exists.");

            context.Entry(existingMember).CurrentValues.SetValues(entity);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            Member? member = context.Members.Find(id);
            if (member == null)
                throw new KeyNotFoundException($"No member found with ID {id}.");

            context.Members.Remove(member);
            context.SaveChanges();
        }
    }
}
