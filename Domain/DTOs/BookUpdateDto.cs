namespace Domain.DTOs
{
    public class BookUpdateDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public bool? IsBorrowed { get; set; }
        public DateTime? BorrowedDate { get; set; }
        public int? BorrowedBy { get; set; }
    }
}
