using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Author { get; set; }

        // ISBN can now be NULL in the database
        public string? ISBN { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalCopies { get; set; }

        [Range(0, int.MaxValue)]
        public int AvailableCopies { get; set; }

        public bool IsAvailable { get; set; } = true;

        // New fields
        [StringLength(200)]
        public string? Publisher { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string? Description { get; set; }

        public int? PageCount { get; set; }

        [StringLength(20)]
        public string? Language { get; set; }

        public decimal? AverageRating { get; set; }

        public int? RatingsCount { get; set; }

        public string? Thumbnail { get; set; }

        public string? GoogleBookId { get; set; }

        public string Status => IsAvailable ? "Available" : "Issued";
    }
}