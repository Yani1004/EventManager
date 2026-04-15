namespace EventManager.Models
{
	public class UserListItem
	{
		public string Id { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? RequestedRole { get; set; }
		public bool IsApproved { get; set; }
		public string CurrentRole { get; set; } = string.Empty;
	}
}