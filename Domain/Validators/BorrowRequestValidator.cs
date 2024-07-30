using Domain.DTOs;
using FluentValidation;

namespace Domain.Validators
{
    public class BorrowRequestValidator : AbstractValidator<BorrowRequestDto>
    {
        public BorrowRequestValidator()
        {
            RuleFor(x => x.BookId).GreaterThan(0).WithMessage("Invalid Book ID.");
            RuleFor(x => x.MemberId).GreaterThan(0).WithMessage("Invalid Member ID");
        }
    }
}