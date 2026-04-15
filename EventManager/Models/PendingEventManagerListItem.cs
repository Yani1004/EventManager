namespace EventManager.Models
{
	public class PendingEventManagerListItem
	{
		public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string RequestedRole { get; set; } = string.Empty;
		public bool IsApproved { get; set; }
	}
}