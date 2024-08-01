using FluentValidation;

namespace LiMS.API.Models
{
    public class BorrowReturnModel
    {
        public int BookId { get; set; }
        public int? MemberId { get; set; }

        public class Validator : AbstractValidator<BorrowReturnModel>
        {
            public Validator()
            {
                RuleFor(x => x.BookId)
                    .NotEmpty().WithMessage("Book ID is required.");

                RuleFor(x => x.MemberId)
                    .NotEmpty().WithMessage("Member ID is required.");
            }
        }
    }
}