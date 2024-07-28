using Domain;
using Infrastructure;

namespace LiMS.Tests.Infrastructure
{
    public class MemberRepositoryTests
    {
        private readonly string testFile = "C:\\Users\\Ahmad-Elwan\\source\\repos\\LiMS\\LiMS.Tests\\test_members.json";
        private MemberRepository _repository;

        public MemberRepositoryTests()
        {
            File.WriteAllText(testFile, "[]");
            _repository = new MemberRepository(testFile);
        }

        [Fact]
        public void Add_ShouldAddMember()
        {
            var member = new Member { MemberID = 1, Name = "Test Member", Email = "test@email.com" };
            _repository.Add(member);

            var members = _repository.GetAll();
            Assert.Single(members);
            Assert.Equal(member.Name, members[0].Name);
        }

        [Fact]
        public void Update_ShouldUpdateMember()
        {
            var member = new Member { MemberID = 1, Name = "Test Member", Email = "test@email.com" };
            _repository.Add(member);

            member.Name = "Updated Name";
            _repository.Update(member);

            var updatedMember = _repository.GetById(1);
            Assert.NotNull(updatedMember);
            Assert.Equal("Updated Name", updatedMember.Name);
        }

        [Fact]
        public void Delete_ShouldRemoveMember()
        {
            var member = new Member { MemberID = 1, Name = "Test Member", Email = "test@email.com" };
            _repository.Add(member);
            _repository.Delete(1);

            List<Member> members = _repository.GetAll();

            Assert.DoesNotContain(members, m => m.MemberID == 1);
        }

        [Fact]
        public void GetAll_ShouldReturnAllMembers()
        {
            var members = new List<Member>
            {
                new Member { MemberID = 1, Name = "Member 1", Email = "member1@email.com" },
                new Member { MemberID = 2, Name = "Member 2", Email = "member2@email.com" }
            };

            foreach (var member in members)
            {
                _repository.Add(member);
            }

            var allMembers = _repository.GetAll();
            Assert.Equal(2, allMembers.Count);
        }

        [Fact]
        public void GetById_ShouldReturnMemberById()
        {
            var member = new Member { MemberID = 1, Name = "Test Member", Email = "test@email.com" };
            _repository.Add(member);

            var retrievedMember = _repository.GetById(1);
            Assert.NotNull(retrievedMember);
            Assert.Equal(member.Name, retrievedMember.Name);
        }
    }
}
