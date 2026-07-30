using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.MVC.Models
{
    public class Fine
    {
        [Key]
        public int FineId { get; set; }

        [Required]
        [ForeignKey("BorrowRecord")]
        public int BorrowId { get; set; }
        public BorrowRecord BorrowRecord { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Reason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime GeneratedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PaidDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // "Pending", "Paid"
    }
}
