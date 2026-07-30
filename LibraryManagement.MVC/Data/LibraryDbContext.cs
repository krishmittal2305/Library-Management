using Microsoft.EntityFrameworkCore;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Publication> Publications { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Librarian> Librarians { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<Newspaper> Newspapers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BorrowRecord>()
                .HasOne(b => b.IssuedByLibrarian)
                .WithMany()
                .HasForeignKey(b => b.IssuedByLibrarianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BorrowRecord>()
                .HasOne(b => b.ReturnedByLibrarian)
                .WithMany()
                .HasForeignKey(b => b.ReturnedByLibrarianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
