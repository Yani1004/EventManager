namespace EventManager.Models
{
	public class TicketDetailsDto
	{
		public int RegistrationId { get; set; }
		public int EventId { get; set; }
		public string EventTitle { get; set; } = string.Empty;
		public string EventLocation { get; set; } = string.Empty;
		public DateTime EventDate { get; set; }
		public decimal Price { get; set; }
		public DateTime RegisteredAt { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public string? TicketNumber { get; set; }
		public string? VerificationCode { get; set; }
	}
}
