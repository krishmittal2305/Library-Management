using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.Models
{
    public class Magazine
    {
        [Key]
        public int MagazineId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Publisher { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Language { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        public bool Availability { get; set; } = true;

        [StringLength(1000)]
        public string Description { get; set; }
    }
}
