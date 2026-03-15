using System.ComponentModel.DataAnnotations;

namespace EventManager.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Location must be between 2 and 200 characters.")]
        public string Location { get; set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        [Range(0, 100000, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string CreatorId { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid image URL.")]
        public string? ImageUrl { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

    }
}