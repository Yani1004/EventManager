namespace EventManager.Models
{
	public class EventRegistrationListItem
	{
		public string UserName { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public DateTime RegisteredAt { get; set; }
	}
}