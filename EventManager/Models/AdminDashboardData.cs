namespace EventManager.Models
{
	public class AdminDashboardData
	{
		public int TotalUsers { get; set; }
		public int TotalEventManagers { get; set; }
		public int PendingApprovals { get; set; }
		public int TotalEvents { get; set; }

		public List<ApplicationUser> RecentPendingUsers { get; set; } = new();
	}
}