using Application;
using Domain;

namespace LiMS.API.Routes
{
    public static class MembersRoutes
    {
        public static void MapMemberRoutes(this WebApplication app)
        {
            app.MapGet("/api/members", (LibraryService libraryService) =>
            {
                try
                {
                    List<Member> members = libraryService.GetAllMembers();
                    return Results.Ok(members);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving members: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving members.");
                }
            });

            app.MapGet("/api/members/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Member member = libraryService.GetMemberById(id);
                    return member != null ? Results.Ok(member) : Results.NotFound("Member not found.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving member with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving the member.");
                }
            });

            app.MapPost("/api/members", (Member member, LibraryService libraryService) =>
            {
                try
                {
                    if (member == null)
                        return Results.BadRequest("Invalid member data.");

                    libraryService.AddMember(member);
                    return Results.Created($"/api/members/{member.MemberID}", member);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding member: {ex.Message}");
                    return Results.Problem("An error occurred while adding the member.");
                }
            });

            app.MapPut("/api/members/{id}", (int id, Member member, LibraryService libraryService) =>
            {
                try
                {
                    if (member == null || id != member.MemberID)
                        return Results.BadRequest("Member ID mismatch.");

                    Member existingMember = libraryService.GetMemberById(id);
                    if (existingMember == null)
                        return Results.NotFound("Member not found.");

                    libraryService.UpdateMember(member);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating member with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the member.");
                }
            });

            app.MapDelete("/api/members/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Member member = libraryService.GetMemberById(id);
                    if (member == null)
                        return Results.NotFound("Member not found.");

                    libraryService.DeleteMember(id);
                    return Results.Ok("Member deleted successfully!");
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting member with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while deleting the member.");
                }
            });
        }
    }
}