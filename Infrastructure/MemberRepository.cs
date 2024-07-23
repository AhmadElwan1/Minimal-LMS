using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using LiMS.Domain;

namespace LiMS.Infrastructure
{
    public class MemberRepository : IRepository<Member>
    {
        private readonly string _membersFile;

        public MemberRepository(string membersFile)
        {
            _membersFile = membersFile;
        }

        public List<Member> GetAll()
        {
            if (!File.Exists(_membersFile))
                return new List<Member>();

            string membersJson = File.ReadAllText(_membersFile);
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
            File.WriteAllText(_membersFile, membersJson);
        }
    }
}
