using Application;
using Domain;
using Domain.DTOs;
using FluentValidation;

namespace LiMS.API.Routes
{
    public static class MembersRoutes
    {
        public static void MapMemberRoutes(this WebApplication app)
        {
            app.MapGet("/members", (LibraryService libraryService) =>
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
            })
            .WithTags("Members");

            app.MapGet("/members/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Member member = libraryService.GetMemberById(id);
                    return member != null ? Results.Ok(member) : Results.NotFound(new { Error = "Member not found." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving member with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving the member.");
                }
            })
            .WithTags("Members");

            app.MapPost("/members", async (Member member, LibraryService libraryService, IValidator<Member> validator) =>
            {
                var validationResult = await validator.ValidateAsync(member);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    libraryService.AddMember(member);
                    return Results.Created($"/members/{member.MemberID}", member);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding member: {ex.Message}");
                    return Results.Problem("An error occurred while adding the member.");
                }
            })
            .WithTags("Members");

            app.MapPut("/members/{id}", async (int id, Member member, LibraryService libraryService, IValidator<Member> validator) =>
            {
                var validationResult = await validator.ValidateAsync(member);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    if (member == null || id != member.MemberID)
                        return Results.BadRequest(new { Error = "Member ID mismatch." });

                    Member existingMember = libraryService.GetMemberById(id);
                    if (existingMember == null)
                        return Results.NotFound(new { Error = "Member not found." });

                    libraryService.UpdateMember(member);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating member with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the member.");
                }
            })
            .WithTags("Members");

            app.MapPatch("/members/{id}", async (int id, MemberUpdateDto updateDto, LibraryService libraryService, IValidator<Member> validator) =>
            {
                if (updateDto == null)
                    return Results.BadRequest(new { Error = "Invalid update data." });

                try
                {
                    Member existingMember = libraryService.GetMemberById(id);
                    if (existingMember == null)
                        return Results.NotFound(new { Error = "Member not found." });

                    if (updateDto.Name != null)
                        existingMember.Name = updateDto.Name;

                    if (updateDto.Email != null)
                        existingMember.Email = updateDto.Email;

                    var validationResult = await validator.ValidateAsync(existingMember);
                    if (!validationResult.IsValid)
                        return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                    libraryService.UpdateMember(existingMember);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating member with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while updating the member.");
                }
            })
            .WithTags("Members");

            app.MapDelete("/members/{id}", (int id, LibraryService libraryService) =>
            {
                try
                {
                    Member member = libraryService.GetMemberById(id);
                    if (member == null)
                        return Results.NotFound(new { Error = "Member not found." });

                    libraryService.DeleteMember(id);
                    return Results.Ok(new { Message = "Member deleted successfully!" });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting member with ID {id}: {ex.Message}");
                    return Results.Problem("An error occurred while deleting the member.");
                }
            })
            .WithTags("Members");
        }
    }
}
