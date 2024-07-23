using LiMS.Domain;
using LiMS.Infrastructure;
using Newtonsoft.Json;

namespace LiMS.Tests.Infrastructure
{
    public class MemberRepositoryTests : IDisposable
    {
        private readonly string _tempFilePath;
        private readonly MemberRepository _memberRepository;

        public MemberRepositoryTests()
        {
            _tempFilePath = Path.GetTempFileName();
            _memberRepository = new MemberRepository(_tempFilePath);
        }

        public void Dispose()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        [Fact]
        public void GetAll_Should_Return_Empty_List_When_File_Not_Found()
        {
            // Arrange
            File.Delete(_tempFilePath); 

            // Act
            List<Member> actualMembers = _memberRepository.GetAll();

            // Assert
            Assert.Empty(actualMembers);
        }

        [Fact]
        public void GetAll_Should_Return_All_Members()
        {
            // Arrange
            List<Member> expectedMembers = new List<Member>
            {
                new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" },
                new Member { MemberID = 2, Name = "Jane Smith", Email = "jane@example.com" },
                new Member { MemberID = 3, Name = "Michael Johnson", Email = "michael@example.com" }
            };

            SaveMembersToFile(expectedMembers);

            // Act
            List<Member> actualMembers = _memberRepository.GetAll();

            // Assert
            Assert.Equal(expectedMembers.Count, actualMembers.Count);
            foreach (var expectedMember in expectedMembers)
            {
                var actualMember = actualMembers.Single(m => m.MemberID == expectedMember.MemberID);
                Assert.Equal(expectedMember.Name, actualMember.Name);
                Assert.Equal(expectedMember.Email, actualMember.Email);
            }
        }

        [Fact]
        public void GetById_Should_Return_Correct_Member()
        {
            // Arrange
            List<Member> members = new List<Member>
            {
                new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" },
                new Member { MemberID = 2, Name = "Jane Smith", Email = "jane@example.com" },
                new Member { MemberID = 3, Name = "Michael Johnson", Email = "michael@example.com" }
            };
            SaveMembersToFile(members);

            // Act
            Member foundMember = _memberRepository.GetById(2);

            // Assert
            Assert.NotNull(foundMember);
            Assert.Equal(2, foundMember.MemberID);
            Assert.Equal("Jane Smith", foundMember.Name);
            Assert.Equal("jane@example.com", foundMember.Email);
        }

        [Fact]
        public void Add_Should_Add_New_Member()
        {
            // Arrange
            Member newMember = new Member { MemberID = 4, Name = "New Member", Email = "new@example.com" };

            // Act
            _memberRepository.Add(newMember);

            // Assert
            List<Member> members = GetAllMembersFromFile();
            Member addedMember = members.SingleOrDefault(m => m.MemberID == newMember.MemberID);

            Assert.NotNull(addedMember);
            Assert.Equal(newMember.Name, addedMember.Name);
            Assert.Equal(newMember.Email, addedMember.Email);
        }

        [Fact]
        public void Update_Should_Update_Existing_Member()
        {
            // Arrange
            List<Member> members = new List<Member>
            {
                new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" },
                new Member { MemberID = 2, Name = "Jane Smith", Email = "jane@example.com" },
                new Member { MemberID = 3, Name = "Michael Johnson", Email = "michael@example.com" }
            };
            SaveMembersToFile(members);

            Member updatedMember = new Member { MemberID = 2, Name = "Updated Member", Email = "updated@example.com" };

            // Act
            _memberRepository.Update(updatedMember);

            // Assert
            List<Member> updatedMembers = GetAllMembersFromFile();
            Member foundMember = updatedMembers.Single(m => m.MemberID == updatedMember.MemberID);
            Assert.Equal(updatedMember.Name, foundMember.Name);
            Assert.Equal(updatedMember.Email, foundMember.Email);
        }

        [Fact]
        public void Delete_Should_Delete_Existing_Member()
        {
            // Arrange
            List<Member> members = new List<Member>
            {
                new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" },
                new Member { MemberID = 2, Name = "Jane Smith", Email = "jane@example.com" },
                new Member { MemberID = 3, Name = "Michael Johnson", Email = "michael@example.com" }
            };
            SaveMembersToFile(members);

            // Act
            _memberRepository.Delete(2);

            // Assert
            List<Member> remainingMembers = GetAllMembersFromFile();
            Assert.Equal(2, remainingMembers.Count);
            Assert.DoesNotContain(members.Single(m => m.MemberID == 2), remainingMembers);
        }

        private void SaveMembersToFile(List<Member> members)
        {
            string membersJson = JsonConvert.SerializeObject(members);
            File.WriteAllText(_tempFilePath, membersJson);
        }

        private List<Member> GetAllMembersFromFile()
        {
            if (!File.Exists(_tempFilePath))
                return new List<Member>();

            string membersJson = File.ReadAllText(_tempFilePath);
            return JsonConvert.DeserializeObject<List<Member>>(membersJson);
        }
    }
}
