namespace EventManager.Models
{
	public class AdminEventListItem
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public DateTime Date { get; set; }
		public string Location { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string? AdminNote { get; set; }
		public string CreatorId { get; set; } = string.Empty;
		public string CreatorName { get; set; } = "Unknown";
		public decimal Price { get; set; }
		public string? ImageUrl { get; set; }
		public int Capacity { get; set; }
	}
}