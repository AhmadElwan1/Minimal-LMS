using Domain;
using FluentValidation;

namespace LiMS.API.Models
{
    public class BookModel : Book
    {
        public class Validator : AbstractValidator<BookModel>
        {
            public Validator()
            {
                RuleFor(x => x.BookId)
                    .GreaterThan(0).WithMessage("Book ID must be greater than 0.");

                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Title is required.");

                RuleFor(x => x.Author)
                    .NotEmpty().WithMessage("Author is required.");

                RuleFor(x => x.BorrowedDate)
                    .LessThanOrEqualTo(DateTime.Now).When(x => x.BorrowedDate.HasValue)
                    .WithMessage("Borrowed date cannot be in the future.");

                RuleFor(x => x.IsBorrowed)
                    .Equal(true).When(x => x.BorrowedDate.HasValue)
                    .WithMessage("A book with a borrowed date must be marked as borrowed.")
                    .Equal(false).When(x => !x.BorrowedDate.HasValue)
                    .WithMessage("A book without a borrowed date must not be marked as borrowed.");
            }
        }
    }
}