using System.ComponentModel.DataAnnotations;

namespace EventManager.Models
{
	public class Registration
	{
		public int Id { get; set; }

		[Required]
		public int EventId { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;

		public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

		public int RegistrationStatusId { get; set; } = 1;
		public RegistrationStatus RegistrationStatus { get; set; } = null!;

		public Event Event { get; set; } = null!;
		public ApplicationUser User { get; set; } = null!;

		public Ticket? Ticket { get; set; }
	}
}