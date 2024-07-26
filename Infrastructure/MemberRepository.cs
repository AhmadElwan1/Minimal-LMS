using Newtonsoft.Json;
using LiMS.Domain;

namespace LiMS.Infrastructure
{
    public class MemberRepository(string membersFile) : IRepository<Member>
    {
        public List<Member> GetAll()
        {
            if (!File.Exists(membersFile))
                return [];

            string membersJson = File.ReadAllText(membersFile);
            return JsonConvert.DeserializeObject<List<Member>>(membersJson);
        }

        public Member GetById(int id)
        {
            return GetAll().Find(m => m.MemberID == id);
        }

        public void Add(Member entity)
        {
            List<Member> members = GetAll();
            members.Add(entity);
            SaveChanges(members);
        }

        public void Update(Member entity)
        {
            List<Member> members = GetAll();
            int index = members.FindIndex(m => m.MemberID == entity.MemberID);
            if (index != -1)
            {
                members[index] = entity;
                SaveChanges(members);
            }
        }

        public void Delete(int id)
        {
            List<Member> members = GetAll();
            members.RemoveAll(m => m.MemberID == id);
            SaveChanges(members);
        }

        private void SaveChanges(List<Member> members)
        {
            string membersJson = JsonConvert.SerializeObject(members, Formatting.Indented);
            File.WriteAllText(membersFile, membersJson);
        }
    }
}
