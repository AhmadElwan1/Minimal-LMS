using LiMS.Application;
using LiMS.Domain;

namespace LiMS.Tests
{

    /*
        Note:
 
            Arrange: The part where we create objects and initialize variables.
            Act: The part where the actual execution is done.
            Assert: The part where the verification is done. 
    */

    public class LibraryServiceTests
    {
        private class FakeBookRepository : IRepository<Book>
        {
            private readonly List<Book> _books = new List<Book>();
            private int _nextBookId = 1;

            public List<Book> GetAll()
            {
                return _books;
            }

            public Book GetById(int id)
            {
                return _books.Find(b => b.BookID == id);
            }

            public void Add(Book book)
            {
                book.BookID = _nextBookId++;
                _books.Add(book);
            }

            public void Update(Book book)
            {
                int index = _books.FindIndex(b => b.BookID == book.BookID);
                if (index != -1)
                {
                    _books[index] = book;
                }
            }

            public void Delete(int id)
            {
                _books.RemoveAll(b => b.BookID == id);
            }
        }

        private class FakeMemberRepository : IRepository<Member>
        {
            private readonly List<Member> _members = new List<Member>();
            private int _nextMemberId = 1;

            public List<Member> GetAll()
            {
                return _members;
            }

            public Member GetById(int id)
            {
                return _members.Find(m => m.MemberID == id);
            }

            public void Add(Member member)
            {
                member.MemberID = _nextMemberId++;
                _members.Add(member);
            }

            public void Update(Member member)
            {
                int index = _members.FindIndex(m => m.MemberID == member.MemberID);
                if (index != -1)
                {
                    _members[index] = member;
                }
            }

            public void Delete(int id)
            {
                _members.RemoveAll(m => m.MemberID == id);
            }
        }

        [Fact]
        public void AddBook_Should_Add_New_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Book newBook = new Book { Title = "New Book", Author = "Author" };

            // Act
            libraryService.AddBook(newBook);

            // Assert
            Book addedBook = fakeBookRepository.GetById(newBook.BookID);
            Assert.NotNull(addedBook);
            Assert.Equal(newBook.Title, addedBook.Title);
            Assert.Equal(newBook.Author, addedBook.Author);
        }

        [Fact]
        public void UpdateBook_Should_Update_Existing_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            Book existingBook = new Book { Title = "Existing Book", Author = "Original Author" };
            fakeBookRepository.Add(existingBook);

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            existingBook.Author = "New Author";
            libraryService.UpdateBook(existingBook);

            // Assert
            Book updatedBook = fakeBookRepository.GetById(existingBook.BookID);
            Assert.Equal("New Author", updatedBook.Author);
        }

        [Fact]
        public void DeleteBook_Should_Delete_Existing_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            Book existingBook = new Book { Title = "Existing Book", Author = "Original Author" };
            fakeBookRepository.Add(existingBook);

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            libraryService.DeleteBook(1);

            // Assert
            Book deletedBook = fakeBookRepository.GetById(1);
            Assert.Null(deletedBook);
        }

        [Fact]
        public void AddMember_Should_Add_New_Member()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member newMember = new Member { Name = "New Member", Email = "member@example.com" };

            // Act
            libraryService.AddMember(newMember);

            // Assert
            Member addedMember = fakeMemberRepository.GetById(newMember.MemberID);
            Assert.NotNull(addedMember);
            Assert.Equal(newMember.Name, addedMember.Name);
            Assert.Equal(newMember.Email, addedMember.Email);
        }

        [Fact]
        public void UpdateMember_Should_Update_Existing_Member()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            Member existingMember = new Member { Name = "Existing Member", Email = "original@example.com" };
            fakeMemberRepository.Add(existingMember);

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            existingMember.Email = "updated@example.com";
            libraryService.UpdateMember(existingMember);

            // Assert
            Member updatedMember = fakeMemberRepository.GetById(existingMember.MemberID);
            Assert.Equal("updated@example.com", updatedMember.Email);
        }

        [Fact]
        public void DeleteMember_Should_Delete_Existing_Member()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            Member existingMember = new Member { Name = "Existing Member", Email = "original@example.com" };
            fakeMemberRepository.Add(existingMember);

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            libraryService.DeleteMember(1);

            // Assert
            Member deletedMember = fakeMemberRepository.GetById(1);
            Assert.Null(deletedMember);
        }

        [Fact]
        public void BorrowBook_Should_Borrow_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            Book book = new Book { Title = "Sample Book", IsBorrowed = false };
            Member member = new Member { Name = "John Doe", Email = "john@example.com" };

            fakeBookRepository.Add(book);
            fakeMemberRepository.Add(member);

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            libraryService.BorrowBook(1, 1);

            // Assert
            Book borrowedBook = fakeBookRepository.GetById(1);
            Assert.True(borrowedBook.IsBorrowed);
            Assert.Equal(member.MemberID, borrowedBook.BorrowedBy);
            Assert.NotNull(borrowedBook.BorrowedDate);
        }

        [Fact]
        public void ReturnBook_Should_Return_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            Book borrowedBook = new Book { Title = "Borrowed Book", IsBorrowed = true, BorrowedBy = 1, BorrowedDate = DateTime.Now };
            fakeBookRepository.Add(borrowedBook);

            Member borrowedBy = new Member { Name = "John Doe", Email = "john@example.com" };
            fakeMemberRepository.Add(borrowedBy);

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            libraryService.ReturnBook(1);

            // Assert
            Book returnedBook = fakeBookRepository.GetById(1);

            Assert.False(returnedBook.IsBorrowed);
            Assert.Null(returnedBook.BorrowedBy);
            Assert.Null(returnedBook.BorrowedDate);
        }

        [Fact]
        public void GetAllBooks_Should_Return_All_Books()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            List<Book> books = new List<Book>
            {
                new Book { Title = "Book 1", Author = "Author 1" },
                new Book { Title = "Book 2", Author = "Author 2" }
            };

            foreach (var book in books)
            {
                fakeBookRepository.Add(book);
            }

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            var allBooks = libraryService.GetAllBooks();

            // Assert
            Assert.Equal(2, allBooks.Count);
        }

        [Fact]
        public void GetAllMembers_Should_Return_All_Members()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();

            List<Member> members = new List<Member>
            {
                new Member { Name = "John Doe", Email = "john@example.com" },
                new Member { Name = "Jane Smith", Email = "jane@example.com" }
            };

            foreach (var member in members)
            {
                fakeMemberRepository.Add(member);
            }

            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Act
            var allMembers = libraryService.GetAllMembers();

            // Assert
            Assert.Equal(2, allMembers.Count);
        }
    }
}
