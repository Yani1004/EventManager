namespace EventManager.Models
{
	public class Ticket
	{
		public int Id { get; set; }

		public string TicketNumber { get; set; } = string.Empty;

		public int RegistrationId { get; set; }
		public Registration Registration { get; set; } = null!;

		public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

		public bool IsUsed { get; set; } = false;

		public string? VerificationCode { get; set; }
	}
}