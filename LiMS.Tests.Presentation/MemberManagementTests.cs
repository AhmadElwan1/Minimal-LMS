using Application;
using Domain;
using Moq;
using Presentation;
using System;
using System.Collections.Generic;
using Xunit;

namespace LiMS.Tests.Presentation
{
    public class MemberManagementTests
    {
        private readonly Mock<LibraryService> _mockLibraryService;
        private readonly MemberManagement _memberManagement;

        public MemberManagementTests()
        {
            _mockLibraryService = new Mock<LibraryService>(
                Mock.Of<IRepository<Book>>(),
                Mock.Of<IRepository<Member>>()
            );
            _memberManagement = new MemberManagement();
        }

        [Fact]
        public void AddNewMember_ShouldAddMemberSuccessfully()
        {
            // Arrange
            _mockLibraryService.Setup(l => l.GetAllMembers()).Returns(new List<Member>());

            var input = "Name\nEmail\n";
            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Act
            _memberManagement.AddNewMember(_mockLibraryService.Object);

            // Assert
            _mockLibraryService.Verify(l => l.AddMember(It.IsAny<Member>()), Times.Once);
        }

        [Theory]
        [InlineData("\nEmail\n", "Name cannot be empty. Please enter a valid name.")]
        [InlineData("Name\n\n", "Email cannot be empty. Please enter a valid email.")]
        [InlineData("Name\nduplicate@example.com\n",
            "Email 'duplicate@example.com' is already in use. Please enter a different email.")]
        public void AddNewMember_ShouldHandleVariousInputs(string input, string expectedMessage)
        {
            // Arrange
            var existingMember = new Member { MemberID = 1, Name = "Existing Member", Email = "duplicate@example.com" };
            _mockLibraryService.Setup(l => l.GetAllMembers()).Returns(new List<Member> { existingMember });

            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.AddNewMember(_mockLibraryService.Object);

            // Assert
            var output = stringWriter.ToString().Trim();
            Assert.Contains(expectedMessage, output);

            // Verify that AddMember was not called for invalid cases
            if (expectedMessage != "Member added successfully!")
            {
                _mockLibraryService.Verify(l => l.AddMember(It.IsAny<Member>()), Times.Never);
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
            _mockLibraryService.Setup(l => l.GetMemberById(memberId.GetValueOrDefault())).Returns(originalMember);

            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.UpdateMember(_mockLibraryService.Object);

            // Assert
            if (shouldUpdate)
            {
                // Verify that UpdateMember was called with the correct member details
                if (originalMember != null)
                {
                    _mockLibraryService.Verify(l => l.UpdateMember(It.Is<Member>(m =>
                        m.MemberID == memberId &&
                        m.Name == (expectedName ?? originalMember.Name) &&
                        m.Email == (expectedEmail ?? originalMember.Email)
                    )), Times.Once);
                }
            }
            else
            {
                // Verify that UpdateMember was not called for invalid cases
                _mockLibraryService.Verify(l => l.UpdateMember(It.IsAny<Member>()), Times.Never);

                // Check for the expected error message
                if (memberId == null || !memberId.HasValue || memberId.Value == 2)
                {
                    Assert.Contains("Invalid input. Please enter a valid member ID.", stringWriter.ToString());
                }
                else if (memberId.Value != 1)
                {
                    Assert.Contains("ID not found.", stringWriter.ToString());
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
                    // Simulate that the member exists
                    _mockLibraryService.Setup(l => l.GetMemberById(memberId.Value)).Returns(new Member { MemberID = memberId.Value });
                }
                else
                {
                    // Simulate that the member does not exist
                    _mockLibraryService.Setup(l => l.GetMemberById(memberId.Value)).Returns((Member)null);
                }
            }
            else
            {
                // Simulate invalid ID input
                _mockLibraryService.Setup(l => l.GetMemberById(It.IsAny<int>())).Returns((Member)null);
            }

            // Set up DeleteMember behavior
            if (shouldDelete)
            {
                _mockLibraryService.Setup(l => l.DeleteMember(memberId.Value));
            }
            else
            {
                _mockLibraryService.Setup(l => l.DeleteMember(It.IsAny<int>())).Verifiable(); // Setup but will not verify the call
            }

            var stringReader = new System.IO.StringReader(input);
            Console.SetIn(stringReader);

            // Setup to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.DeleteMember(_mockLibraryService.Object);

            // Assert
            if (shouldDelete)
            {
                // Verify that DeleteMember was called with the correct member ID
                _mockLibraryService.Verify(l => l.DeleteMember(It.Is<int>(id => id == memberId)), Times.Once);
                Assert.Contains("Member deleted successfully!", stringWriter.ToString());
            }
            else
            {
                // Verify that DeleteMember was not called for invalid cases or not found cases
                _mockLibraryService.Verify(l => l.DeleteMember(It.IsAny<int>()), Times.Never);
                if (memberId == null)
                {
                    Assert.Contains("Invalid input. Please enter a valid member ID.", stringWriter.ToString());
                }
                else
                {
                    Assert.Contains("Member not found. Please enter a valid member ID.", stringWriter.ToString());
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
            _mockLibraryService.Setup(l => l.GetAllMembers()).Returns(members);

            // Set up to capture console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _memberManagement.ViewAllMembers(_mockLibraryService.Object);

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
                Assert.Contains("No members found.", output); // Ensure correct message is present
                Assert.DoesNotContain("ID:", output); // Ensure no member details are present
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
            _memberManagement.ManageMembers(_mockLibraryService.Object);

            // Assert
            if (shouldAdd)
            {
                _mockLibraryService.Verify(l => l.AddMember(It.IsAny<Member>()), Times.Once);
                Assert.Contains("Enter details for the new member:", stringWriter.ToString());
            }
            else
            {
                _mockLibraryService.Verify(l => l.AddMember(It.IsAny<Member>()), Times.Never);
            }

            if (shouldUpdate)
            {
                _mockLibraryService.Verify(l => l.UpdateMember(It.IsAny<Member>()), Times.Once);
                Assert.Contains("Enter ID of the member to update:", stringWriter.ToString());
            }
            else
            {
                _mockLibraryService.Verify(l => l.UpdateMember(It.IsAny<Member>()), Times.Never);
            }

            if (shouldDelete)
            {
                _mockLibraryService.Verify(l => l.DeleteMember(It.IsAny<int>()), Times.Once);
                Assert.Contains("Enter ID of the member to delete:", stringWriter.ToString());
            }
            else
            {
                _mockLibraryService.Verify(l => l.DeleteMember(It.IsAny<int>()), Times.Never);
            }

            if (shouldView)
            {
                _mockLibraryService.Verify(l => l.GetAllMembers(), Times.Once);
                Assert.Contains("===== All Members =====", stringWriter.ToString());
            }
            else
            {
                _mockLibraryService.Verify(l => l.GetAllMembers(), Times.Never);
            }

            if (input == "5")
            {
                Assert.Contains("===== Manage Members =====", stringWriter.ToString());
            }
            else if (input == "invalid")
            {
                Assert.Contains("Invalid input. Please enter a number from 1 to 5.", stringWriter.ToString());
            }
        }




    }
}
