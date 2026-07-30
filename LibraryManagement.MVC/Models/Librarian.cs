using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.Models
{
    public class Librarian
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Shift { get; set; } // e.g. Morning, Evening
    }
}
