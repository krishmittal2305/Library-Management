using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        
        public int? BookId { get; set; }
        public Book Book { get; set; }

        public int? PublicationId { get; set; }
        public Publication Publication { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }
        
        [Required]
        public DateTime BorrowDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public int? IssuedByLibrarianId { get; set; }
        public Librarian IssuedByLibrarian { get; set; }

        public int? ReturnedByLibrarianId { get; set; }
        public Librarian ReturnedByLibrarian { get; set; }
    }
}
