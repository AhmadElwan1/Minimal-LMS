using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(b => b.BookId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Book>()
                .HasOne(b => b.BorrowedByMember)
                .WithMany()
                .HasForeignKey(b => b.BorrowedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Member>()
                .Property(m => m.MemberID)
                .ValueGeneratedOnAdd();
        }
    }
}