using System;
using Xunit;
using Moq;
using LiMS.Application;
using LiMS.Domain;
using System.Collections.Generic;
using Presentation;

namespace LiMS.Tests.Presentation
{
    public class MemberManagementTests
    {
        private readonly Mock<IConsole> _mockConsole = new();

        [Fact]
        public void AddNewMember_ValidInput_ShouldAddMember()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var newMember = new Member
            {
                MemberID = 1,
                Name = "Test Member",
                Email = "test@example.com"
            };

            mockLibraryService.Setup(s => s.GetAllMembers()).Returns(new List<Member>());
            mockLibraryService.Setup(s => s.AddMember(newMember));

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("Test Member")  // Name
                .Returns("test@example.com")  // Email
                .Returns("5"); // Exit after adding member
            Console.SetIn(_mockConsole.Object.In);

            // Act
            MemberManagement.AddNewMember(mockLibraryService.Object);

            // Assert
            mockLibraryService.Verify(s => s.AddMember(newMember), Times.Once);
        }

        [Fact]
        public void UpdateMember_ValidInput_ShouldUpdateMember()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var existingMember = new Member
            {
                MemberID = 1,
                Name = "Existing Member",
                Email = "existing@example.com"
            };

            mockLibraryService.Setup(s => s.GetMemberById(1)).Returns(existingMember);
            mockLibraryService.Setup(s => s.UpdateMember(existingMember));

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("")  // No change to name
                .Returns("newemail@example.com")  // New email
                .Returns("5"); // Exit after updating member
            Console.SetIn(_mockConsole.Object.In);

            // Act
            MemberManagement.UpdateMember(mockLibraryService.Object);

            // Assert
            Assert.Equal("Existing Member", existingMember.Name);  // Name should not change
            Assert.Equal("newemail@example.com", existingMember.Email);  // Email should update
            mockLibraryService.Verify(s => s.UpdateMember(existingMember), Times.Once);
        }

        [Fact]
        public void DeleteMember_ValidInput_ShouldDeleteMember()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var memberIdToDelete = 1;

            mockLibraryService.Setup(s => s.DeleteMember(memberIdToDelete));

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns(memberIdToDelete.ToString())  // Member ID to delete
                .Returns("5"); // Exit after deleting member
            Console.SetIn(_mockConsole.Object.In);

            // Act
            MemberManagement.DeleteMember(mockLibraryService.Object);

            // Assert
            mockLibraryService.Verify(s => s.DeleteMember(memberIdToDelete), Times.Once);
        }

        [Fact]
        public void ViewAllMembers_ShouldDisplayAllMembers()
        {
            // Arrange
            var mockLibraryService = new Mock<LibraryService>();
            var program = new Program(mockLibraryService.Object);

            var members = new List<Member>
            {
                new() { MemberID = 1, Name = "Member 1", Email = "member1@example.com" },
                new() { MemberID = 2, Name = "Member 2", Email = "member2@example.com" }
            };

            mockLibraryService.Setup(s => s.GetAllMembers()).Returns(members);

            _mockConsole.Setup(c => c.WriteLine(It.IsAny<string>()));

            Console.SetOut(_mockConsole.Object.Out);

            // Act
            MemberManagement.ViewAllMembers(mockLibraryService.Object);

            // Assert
            _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Exactly(members.Count));
        }
    }
}
