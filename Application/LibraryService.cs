using Domain;
using Domain.DTOs;
using FluentValidation;

namespace Application
{
    public class LibraryService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Member> _memberRepository;
        private readonly IValidator<Book> _bookValidator;
        private readonly IValidator<Member> _memberValidator;
        private readonly IValidator<BorrowRequestDto> _borrowRequestValidator;

        public LibraryService(
            IRepository<Book> bookRepository,
            IRepository<Member> memberRepository,
            IValidator<Book> bookValidator,
            IValidator<Member> memberValidator,
            IValidator<BorrowRequestDto> borrowRequestValidator)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _bookValidator = bookValidator;
            _memberValidator = memberValidator;
            _borrowRequestValidator = borrowRequestValidator;
        }

        // Generic Validation Method
        private void Validate<T>(IValidator<T> validator, T entity)
        {
            var validationResult = validator.Validate(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        // Generic Add, Update, Delete Operations
        private void AddEntity<T>(IRepository<T> repository, T entity) => repository.Add(entity);
        private void UpdateEntity<T>(IRepository<T> repository, T entity) => repository.Update(entity);
        private void DeleteEntity<T>(IRepository<T> repository, int id) => repository.Delete(id);

        public void AddBook(Book book)
        {
            Validate(_bookValidator, book);
            AddEntity(_bookRepository, book);
        }

        public void UpdateBook(Book book)
        {
            Validate(_bookValidator, book);
            UpdateEntity(_bookRepository, book);
        }

        public void DeleteBook(int bookID) => DeleteEntity(_bookRepository, bookID);
        public List<Book> GetAllBooks() => _bookRepository.GetAll();
        public Book GetBookById(int bookID) => _bookRepository.GetById(bookID);

        public void AddMember(Member member)
        {
            Validate(_memberValidator, member);
            AddEntity(_memberRepository, member);
        }

        public void UpdateMember(Member member)
        {
            Validate(_memberValidator, member);
            UpdateEntity(_memberRepository, member);
        }

        public void DeleteMember(int memberID) => DeleteEntity(_memberRepository, memberID);
        public List<Member> GetAllMembers() => _memberRepository.GetAll();
        public Member GetMemberById(int memberID) => _memberRepository.GetById(memberID);

        // Borrowing operations
        public void BorrowBook(BorrowRequestDto request)
        {
            Validate(_borrowRequestValidator, request);

            var book = _bookRepository.GetById(request.BookId);
            var member = _memberRepository.GetById(request.MemberId);

            if (book == null)
                throw new ArgumentException($"No book found with ID {request.BookId}");

            if (member == null)
                throw new ArgumentException($"No member found with ID {request.MemberId}");

            if (book.IsBorrowed)
                throw new InvalidOperationException("This book is already borrowed.");

            book.IsBorrowed = true;
            book.BorrowedDate = DateTime.Now;
            book.BorrowedBy = request.MemberId;

            UpdateEntity(_bookRepository, book);
        }

        public void ReturnBook(int bookID)
        {
            var book = _bookRepository.GetById(bookID);

            if (book == null)
                throw new ArgumentException($"No book found with ID {bookID}");

            if (!book.IsBorrowed)
                throw new InvalidOperationException("This book is not currently borrowed.");

            book.IsBorrowed = false;
            book.BorrowedDate = null;
            book.BorrowedBy = null;

            UpdateEntity(_bookRepository, book);
        }

        public List<Book> GetAllBorrowedBooks() => _bookRepository.GetAll().Where(b => b.IsBorrowed).ToList();
    }
}
