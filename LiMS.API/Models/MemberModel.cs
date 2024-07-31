using Domain;
using FluentValidation;

namespace LiMS.API.Models;

public class MemberModel : Member
{
    public class Validator : AbstractValidator<MemberModel>
    {
        public Validator()
        {
            RuleFor(x => x.MemberID)
                .GreaterThan(0).WithMessage("Member ID must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.");
        }
    }
}