using Application;
using Domain;
using FluentValidation.Results;

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
                    List<MemberModel> memberModels = members.Select(m => new MemberModel
                    {
                        MemberID = m.MemberID,
                        Name = m.Name,
                        Email = m.Email
                    }).ToList();

                    return Results.Ok(memberModels);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving members: {ex.Message}");
                    return Results.Problem("An error occurred while retrieving members.");
                }
            }).WithTags("Members");

            app.MapPost("/members", (MemberModel memberModel, LibraryService libraryService) =>
            {
                MemberModel.Validator validator = new MemberModel.Validator();

                ValidationResult validationResult = validator.Validate(memberModel);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    Member member = new Member
                    {
                        MemberID = memberModel.MemberID,
                        Name = memberModel.Name,
                        Email = memberModel.Email
                    };

                    libraryService.AddMember(member);
                    return Results.Created($"/members/{member.MemberID}", memberModel);
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
            }).WithTags("Members");

            app.MapPut("/members/{id}", (int id, MemberModel memberModel, LibraryService libraryService) =>
            {
                MemberModel.Validator validator = new MemberModel.Validator();

                ValidationResult validationResult = validator.Validate(memberModel);
                if (!validationResult.IsValid)
                    return Results.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

                try
                {
                    if (id != memberModel.MemberID)
                        return Results.BadRequest(new { Error = "Member ID mismatch." });

                    Member existingMember = libraryService.GetMemberById(id);
                    if (existingMember == null)
                        return Results.NotFound(new { Error = "Member not found." });

                    existingMember.Name = memberModel.Name;
                    existingMember.Email = memberModel.Email;

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
            }).WithTags("Members");

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
            }).WithTags("Members");
        }
    }
}
