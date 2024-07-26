namespace LiMS.Domain
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsBorrowed { get; set; }
        public DateTime? BorrowedDate { get; set; }
        public int? BorrowedBy { get; set; }
    }
}
