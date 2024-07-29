using Application;
using Moq;
using Presentation;
using System;
using System.Collections.Generic;
using Domain;
using Xunit;

namespace LiMS.Tests.Presentation
{
    public class MemberManagementTests
    {
        private readonly Mock<IRepository<Member>> _mockMemberRepo;
        private readonly Mock<IRepository<Book>> _mockBookRepo;
        private readonly LibraryService _libraryService;
        private readonly MemberManagement _memberManagement;

        public MemberManagementTests()
        {
            // Create mocks for repositories
            _mockMemberRepo = new Mock<IRepository<Member>>();
            _mockBookRepo = new Mock<IRepository<Book>>();

            // Instantiate LibraryService with mocked repositories
            _libraryService = new LibraryService(_mockBookRepo.Object, _mockMemberRepo.Object);
            _memberManagement = new MemberManagement();
        }

        [Fact]
        public void AddNewMember_ShouldAddMemberSuccessfully()
        {
            // Arrange
            _mockMemberRepo.Setup(r => r.GetAll()).Returns(new List<Member>());
            var input = "Name\nEmail\n";
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _memberManagement.AddNewMember(_libraryService);

            // Assert
            _mockMemberRepo.Verify(r => r.Add(It.IsAny<Member>()), Times.Once);
        }

        [Theory]
        [InlineData("\nEmail\n", "Name cannot be empty. Please enter a valid name.")]
        [InlineData("Name\n\n", "Email cannot be empty. Please enter a valid email.")]
        [InlineData("Name\nduplicate@example.com\n", "Email 'duplicate@example.com' is already in use. Please enter a different email.")]
        [InlineData("Name\nunique@example.com\n", "Member added successfully!")] // Valid case
        public void AddNewMember_ShouldHandleVariousInputs(string input, string expectedMessage)
        {
            // Arrange
            var existingMember = new Member { MemberID = 1, Name = "Existing Member", Email = "duplicate@example.com" };
            var members = new List<Member> { existingMember };

            // Setup the mock repository to return existing members
            _mockMemberRepo.Setup(r => r.GetAll()).Returns(members);

            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.AddNewMember(_libraryService);

            // Assert
            var output = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, output);

            // Verify that AddMember was not called for invalid cases
            if (expectedMessage != "Member added successfully!")
            {
                _mockMemberRepo.Verify(r => r.Add(It.IsAny<Member>()), Times.Never);
            }
            else
            {
                // Verify that AddMember was called when the case is valid
                _mockMemberRepo.Verify(r => r.Add(It.Is<Member>(m =>
                    m.Name == "Name" &&
                    m.Email == "unique@example.com"
                )), Times.Once);
            }
        }


        [Theory]
        [InlineData("1\nNew Name\nNew Email\n", 1, "New Name", "New Email", true)] // Update all details
        [InlineData("1\nNew Name\n\n", 1, "New Name", "Old Email", true)] // Update name only
        [InlineData("1\n\nNew Email\n", 1, "Old Name", "New Email", true)] // Update email only
        [InlineData("1\n\n\n", 1, "Old Name", "Old Email", true)] // Update none
        [InlineData("invalid\nNew Name\nNew Email\n", null, "Old Name", "Old Email", false)] // Invalid ID
        [InlineData("2\nNew Name\nNew Email\n", 2, null, null, false)] // ID not found
        public void UpdateMember_ShouldHandleVariousInputs(string input, int? memberId, string expectedName,
            string expectedEmail, bool shouldUpdate)
        {
            // Arrange
            var originalMember = memberId.HasValue
                ? new Member { MemberID = memberId.Value, Name = "Old Name", Email = "Old Email" }
                : null;
            _mockMemberRepo.Setup(r => r.GetById(memberId.GetValueOrDefault())).Returns(originalMember);

            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.UpdateMember(_libraryService);

            // Assert
            var output = stringWriter.ToString().Trim();
            if (shouldUpdate)
            {
                // Verify that UpdateMember was called with the correct member details
                if (originalMember != null)
                {
                    _mockMemberRepo.Verify(r => r.Update(It.Is<Member>(m =>
                        m.MemberID == memberId &&
                        m.Name == (expectedName ?? originalMember.Name) &&
                        m.Email == (expectedEmail ?? originalMember.Email)
                    )), Times.Once);
                }
            }
            else
            {
                // Verify that UpdateMember was not called for invalid cases
                _mockMemberRepo.Verify(r => r.Update(It.IsAny<Member>()), Times.Never);

                if (!memberId.HasValue)
                {
                    Assert.Contains("Invalid input. Please enter a valid member ID.", output);
                }
                else
                {
                    Assert.Contains("Member not found. Please enter a valid member ID.", output);
                }
            }
        }

        [Theory]
        [InlineData("1\n", 1, true)] // Valid ID, member exists
        [InlineData("invalid\n", null, false)] // Invalid ID
        [InlineData("2\n", 2, false)] // ID not found
        public void DeleteMember_ShouldHandleVariousInputs(string input, int? memberId, bool shouldDelete)
        {
            // Arrange
            if (memberId.HasValue)
            {
                // Simulate that the member exists or does not exist
                if (shouldDelete)
                {
                    _mockMemberRepo.Setup(r => r.GetById(memberId.Value)).Returns(new Member { MemberID = memberId.Value });
                }
                else
                {
                    _mockMemberRepo.Setup(r => r.GetById(memberId.Value)).Returns((Member)null);
                }
            }
            else
            {
                _mockMemberRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Member)null);
            }

            if (shouldDelete)
            {
                _mockMemberRepo.Setup(r => r.Delete(It.Is<int>(id => id == memberId.Value)));
            }
            else
            {
                _mockMemberRepo.Setup(r => r.Delete(It.IsAny<int>())).Verifiable(); // Setup but will not verify the call
            }

            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.DeleteMember(_libraryService);

            // Assert
            var output = stringWriter.ToString().Trim();
            if (shouldDelete)
            {
                _mockMemberRepo.Verify(r => r.Delete(It.Is<int>(id => id == memberId.Value)), Times.Once);
                Assert.Contains("Member deleted successfully!", output);
            }
            else
            {
                _mockMemberRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
                if (memberId == null)
                {
                    Assert.Contains("Invalid input. Please enter a valid member ID.", output);
                }
                else
                {
                    Assert.Contains("Member not found. Please enter a valid member ID.", output);
                }
            }
        }

        [Theory]
        [InlineData("", "===== All Members =====", false)] // No Members
        [InlineData("1,John Doe,john@example.com", "===== All Members =====", true)] // Single Member
        [InlineData("1,John Doe,john@example.com|2,Jane Smith,jane@example.com", "===== All Members =====", true)] // Multiple Members
        public void ViewAllMembers_ShouldHandleVariousInputs(string memberData, string expectedHeader, bool hasMembers)
        {
            // Arrange
            List<Member> members = new List<Member>();
            if (hasMembers)
            {
                foreach (var data in memberData.Split('|'))
                {
                    var parts = data.Split(',');
                    members.Add(new Member
                    {
                        MemberID = int.Parse(parts[0]),
                        Name = parts[1],
                        Email = parts[2]
                    });
                }
            }
            _mockMemberRepo.Setup(r => r.GetAll()).Returns(members);

            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.ViewAllMembers(_libraryService);

            // Assert
            var output = stringWriter.ToString().Trim();
            Assert.Contains(expectedHeader, output);

            if (hasMembers)
            {
                foreach (var member in members)
                {
                    Assert.Contains($"ID: {member.MemberID}, Name: {member.Name}, Email: {member.Email}", output);
                }
            }
            else
            {
                Assert.Contains("No members found.", output);
                Assert.DoesNotContain("ID:", output);
            }
        }

        [Theory]
        [InlineData("1", true, false, false, false)] // Add new member
        [InlineData("2", false, true, false, false)] // Update member
        [InlineData("3", false, false, true, false)] // Delete member
        [InlineData("4", false, false, false, true)] // View all members
        [InlineData("5", false, false, false, false)] // Back to main menu
        [InlineData("invalid", false, false, false, false)] // Invalid input
        public void ManageMembers_ShouldHandleVariousInputs(string input, bool shouldAdd, bool shouldUpdate, bool shouldDelete, bool shouldView)
        {
            // Arrange
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.ManageMembers(_libraryService);

            // Assert
            var output = stringWriter.ToString().Trim();

            if (shouldAdd)
            {
                _mockMemberRepo.Verify(r => r.Add(It.IsAny<Member>()), Times.Once);
                Assert.Contains("Enter details for the new member:", output);
            }
            else
            {
                _mockMemberRepo.Verify(r => r.Add(It.IsAny<Member>()), Times.Never);
            }

            if (shouldUpdate)
            {
                _mockMemberRepo.Verify(r => r.Update(It.IsAny<Member>()), Times.Once);
                Assert.Contains("Enter ID of the member to update:", output);
            }
            else
            {
                _mockMemberRepo.Verify(r => r.Update(It.IsAny<Member>()), Times.Never);
            }

            if (shouldDelete)
            {
                _mockMemberRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Once);
                Assert.Contains("Enter ID of the member to delete:", output);
            }
            else
            {
                _mockMemberRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            }

            if (shouldView)
            {
                _mockMemberRepo.Verify(r => r.GetAll(), Times.Once);
                Assert.Contains("===== All Members =====", output);
            }
            else
            {
                _mockMemberRepo.Verify(r => r.GetAll(), Times.Never);
            }

            if (input == "5")
            {
                Assert.Contains("===== Manage Members =====", output);
            }
            else if (input == "invalid")
            {
                Assert.Contains("Invalid input. Please enter a number from 1 to 5.", output);
            }
        }
    }
}
