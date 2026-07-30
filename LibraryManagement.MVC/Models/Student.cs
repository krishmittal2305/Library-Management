using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string EnrollmentNo { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Department { get; set; }

        public int Semester { get; set; }
    }
}
