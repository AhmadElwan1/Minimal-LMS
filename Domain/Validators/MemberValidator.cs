using FluentValidation;

namespace Domain.Validators
{
    public class MemberValidator : AbstractValidator<Member>
    {
        public MemberValidator()
        {
            RuleFor(member => member.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(member => member.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(member => member.MemberID)
                .GreaterThan(0).WithMessage("Member ID cannot be 0 or less.");
        }
    }
}