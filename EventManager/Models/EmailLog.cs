namespace EventManager.Models
{
	public class EmailLog
	{
		public int Id { get; set; }

		public string ToEmail { get; set; } = string.Empty;
		public string Subject { get; set; } = string.Empty;
		public string BodyPreview { get; set; } = string.Empty;
		public string EmailType { get; set; } = string.Empty;

		public bool IsSent { get; set; }
		public string? ErrorMessage { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}