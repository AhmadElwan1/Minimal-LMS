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
                    .GreaterThan(0).WithMessage("Invalid Book ID.");

                RuleFor(x => x.MemberId)
                    .GreaterThan(0).WithMessage("Invalid Member ID");
            }
        }
    }
}