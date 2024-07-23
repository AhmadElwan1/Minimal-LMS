using LiMS.Application;
using LiMS.Domain;

namespace LiMS.Tests.Application
{
    public class LibraryServiceTests
    {
        // Fake repositories for testing purposes
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

        // Test cases for LibraryService

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
            Assert.Single(fakeBookRepository.GetAll());
            Assert.Equal(newBook.Title, fakeBookRepository.GetById(newBook.BookID).Title);
            Assert.Equal(newBook.Author, fakeBookRepository.GetById(newBook.BookID).Author);
        }

        [Fact]
        public void UpdateBook_Should_Update_Existing_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Book existingBook = new Book { BookID = 1, Title = "Existing Book", Author = "Original Author" };
            fakeBookRepository.Add(existingBook);

            // Act
            existingBook.Author = "New Author";
            libraryService.UpdateBook(existingBook);

            // Assert
            Assert.Equal("New Author", fakeBookRepository.GetById(existingBook.BookID).Author);
        }

        [Fact]
        public void DeleteBook_Should_Delete_Existing_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Book existingBook = new Book { BookID = 1, Title = "Existing Book", Author = "Original Author" };
            fakeBookRepository.Add(existingBook);

            // Act
            libraryService.DeleteBook(existingBook.BookID);

            // Assert
            Assert.Empty(fakeBookRepository.GetAll());
        }

        [Fact]
        public void AddMember_Should_Add_New_Member()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member newMember = new Member { Name = "New Member", Email = "newmember@example.com" };

            // Act
            libraryService.AddMember(newMember);

            // Assert
            Assert.Single(fakeMemberRepository.GetAll());
            Assert.Equal(newMember.Name, fakeMemberRepository.GetById(newMember.MemberID).Name);
            Assert.Equal(newMember.Email, fakeMemberRepository.GetById(newMember.MemberID).Email);
        }

        [Fact]
        public void UpdateMember_Should_Update_Existing_Member()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member existingMember = new Member { MemberID = 1, Name = "Existing Member", Email = "original@example.com" };
            fakeMemberRepository.Add(existingMember);

            // Act
            existingMember.Email = "updated@example.com";
            libraryService.UpdateMember(existingMember);

            // Assert
            Assert.Equal("updated@example.com", fakeMemberRepository.GetById(existingMember.MemberID).Email);
        }

        [Fact]
        public void DeleteMember_Should_Delete_Existing_Member()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member existingMember = new Member { MemberID = 1, Name = "Existing Member", Email = "original@example.com" };
            fakeMemberRepository.Add(existingMember);

            // Act
            libraryService.DeleteMember(existingMember.MemberID);

            // Assert
            Assert.Empty(fakeMemberRepository.GetAll());
        }

        [Fact]
        public void BorrowBook_Should_Borrow_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member member = new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" };
            Book book = new Book { BookID = 1, Title = "Sample Book", Author = "Ahmad" };
            fakeMemberRepository.Add(member);
            fakeBookRepository.Add(book);

            // Act
            libraryService.BorrowBook(1, 1);

            // Assert
            Book borrowedBook = fakeBookRepository.GetById(1);
            Assert.True(borrowedBook.IsBorrowed);
            Assert.Equal(member.MemberID, borrowedBook.BorrowedBy);
            Assert.NotNull(borrowedBook.BorrowedDate);
        }

        [Fact]
        public void BorrowBook_Should_Print_Error_Message_If_Book_Not_Found()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member member = new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" };
            fakeMemberRepository.Add(member);

            // Capture console output
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                libraryService.BorrowBook(1, 1); // Attempt to borrow a book with ID 1

                // Assert
                string expectedMessage = $"Book with ID 1 not found.{Environment.NewLine}";
                Assert.Equal(expectedMessage, sw.ToString());
            }
        }
        [Fact]
        public void BorrowBook_Should_Print_Message_If_Book_Is_Already_Borrowed()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Add a borrowed book to the repository
            Book borrowedBook = new Book { BookID = 1, Title = "Sample Book", Author = "Ahmad", IsBorrowed = true, BorrowedBy = 1 };
            fakeBookRepository.Add(borrowedBook);

            // Capture console output
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                libraryService.BorrowBook(1, 2); // Try to borrow the already borrowed book

                // Assert
                string expectedMessage = "This book is already borrowed." + Environment.NewLine;
                Assert.Equal(expectedMessage, sw.ToString());
            }
        }

        [Fact]
        public void BorrowBook_Should_Print_Message_If_Member_Not_Found()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Add a book to the repository
            Book book = new Book { BookID = 1, Title = "Sample Book", Author = "Ahmad" };
            fakeBookRepository.Add(book);

            // Capture console output
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                libraryService.BorrowBook(1, 99);

                // Assert
                string expectedMessage = $"Member with ID 99 not found." + Environment.NewLine;
                Assert.Equal(expectedMessage, sw.ToString());
            }
        }

        [Fact]
        public void ReturnBook_Should_Print_Message_If_Book_Not_Found()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Add a borrowed book to the repository
            Book borrowedBook = new Book { BookID = 1, Title = "Sample Book", Author = "Ahmad", IsBorrowed = true, BorrowedBy = 1, BorrowedDate = DateTime.Now };
            fakeBookRepository.Add(borrowedBook);

            // Capture console output
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                libraryService.ReturnBook(2); // Try to return book with ID 2 (which doesn't exist)

                // Assert
                string expectedMessage = $"Book with ID 2 not found." + Environment.NewLine;
                Assert.Equal(expectedMessage, sw.ToString());
            }
        }

        [Fact]
        public void ReturnBook_Should_Return_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member member = new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" };
            Book borrowedBook = new Book { BookID = 1, Title = "Borrowed Book", IsBorrowed = true, BorrowedBy = 1, BorrowedDate = DateTime.Now };
            fakeMemberRepository.Add(member);
            fakeBookRepository.Add(borrowedBook);

            // Act
            libraryService.ReturnBook(1);

            // Assert
            Book returnedBook = fakeBookRepository.GetById(1);
            Assert.False(returnedBook.IsBorrowed);
            Assert.Null(returnedBook.BorrowedBy);
            Assert.Null(returnedBook.BorrowedDate);
        }

        [Fact]
        public void ReturnBook_Should_Print_Message_If_Book_Not_Borrowed()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Add a book to the repository (not borrowed)
            Book book = new Book { BookID = 1, Title = "Sample Book", Author = "Ahmad" };
            fakeBookRepository.Add(book);

            // Capture console output
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                libraryService.ReturnBook(1); // Try to return book with ID 1 (not borrowed)

                // Assert
                string expectedMessage = "This book is not currently borrowed." + Environment.NewLine;
                Assert.Equal(expectedMessage, sw.ToString());
            }
        }

        [Fact]
        public void GetAllBooks_Should_Return_All_Books()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Book book1 = new Book { BookID = 1, Title = "Book 1", Author = "Author 1" };
            Book book2 = new Book { BookID = 2, Title = "Book 2", Author = "Author 2" };
            fakeBookRepository.Add(book1);
            fakeBookRepository.Add(book2);

            // Act
            var allBooks = libraryService.GetAllBooks();

            // Assert
            Assert.Equal(2, allBooks.Count);
        }

        [Fact]
        public void GetAllBorrowedBooks_Should_Return_All_Borrowed_Books()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);

            // Add borrowed and non-borrowed books to the repository
            Book borrowedBook1 = new Book { BookID = 1, Title = "Borrowed Book 1", Author = "Author 1", IsBorrowed = true, BorrowedBy = 1, BorrowedDate = DateTime.Now };
            Book borrowedBook2 = new Book { BookID = 2, Title = "Borrowed Book 2", Author = "Author 2", IsBorrowed = true, BorrowedBy = 2, BorrowedDate = DateTime.Now };
            Book nonBorrowedBook = new Book { BookID = 3, Title = "Non-Borrowed Book", Author = "Author 3", IsBorrowed = false };

            fakeBookRepository.Add(borrowedBook1);
            fakeBookRepository.Add(borrowedBook2);
            fakeBookRepository.Add(nonBorrowedBook);

            // Act
            List<Book> borrowedBooks = libraryService.GetAllBorrowedBooks();

            // Assert
            Assert.Equal(2, borrowedBooks.Count); // Ensure only borrowed books are returned
            Assert.Contains(borrowedBook1, borrowedBooks);
            Assert.Contains(borrowedBook2, borrowedBooks);
        }

        [Fact]
        public void GetAllMembers_Should_Return_All_Members()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member member1 = new Member { MemberID = 1, Name = "John Doe", Email = "john@example.com" };
            Member member2 = new Member { MemberID = 2, Name = "Jane Smith", Email = "jane@example.com" };
            fakeMemberRepository.Add(member1);
            fakeMemberRepository.Add(member2);

            // Act
            var allMembers = libraryService.GetAllMembers();

            // Assert
            Assert.Equal(2, allMembers.Count);
        }

        [Fact]
        public void GetBookById_Should_Return_Correct_Book()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Book expectedBook = new Book { BookID = 1, Title = "Test Book", Author = "Test Author" };
            fakeBookRepository.Add(expectedBook);

            // Act
            Book actualBook = libraryService.GetBookById(1);

            // Assert
            Assert.NotNull(actualBook);
            Assert.Equal(expectedBook.BookID, actualBook.BookID);
            Assert.Equal(expectedBook.Title, actualBook.Title);
            Assert.Equal(expectedBook.Author, actualBook.Author);
        }

        [Fact]
        public void GetMemberById_Should_Return_Correct_Member()
        {
            // Arrange
            FakeBookRepository fakeBookRepository = new FakeBookRepository();
            FakeMemberRepository fakeMemberRepository = new FakeMemberRepository();
            LibraryService libraryService = new LibraryService(fakeBookRepository, fakeMemberRepository);
            Member expectedMember = new Member { MemberID = 1, Name = "Test Name", Email = "Test Email" };
            fakeMemberRepository.Add(expectedMember);

            // Act
            Member actualMember = libraryService.GetMemberById(1);

            // Assert
            Assert.NotNull(actualMember);
            Assert.Equal(expectedMember.MemberID, actualMember.MemberID);
            Assert.Equal(expectedMember.Name, actualMember.Name);
            Assert.Equal(expectedMember.Email, actualMember.Email);
        }
    }
}
