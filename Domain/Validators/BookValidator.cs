using FluentValidation;

namespace Domain.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(book => book.Title)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(book => book.Author)
                .NotEmpty().WithMessage("Author is required.");

            RuleFor(book => book.BookId)
                .GreaterThan(0).WithMessage("Book ID cannot be 0 or less.");

            RuleFor(book => book.BorrowedDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Borrowed date cannot be in the future.");

            RuleFor(book => book.IsBorrowed)
                .Equal(true).When(book => book.BorrowedDate.HasValue).WithMessage("A book with a borrowed date must be marked as borrowed.")
                .Equal(false).When(book => !book.BorrowedDate.HasValue).WithMessage("A book without a borrowed date must not be marked as borrowed.");
        }
    }
}
