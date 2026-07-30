using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.Models
{
    public class Publication
    {
        public int Id { get; set; }

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
        public PublicationType Type { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
