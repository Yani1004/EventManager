namespace EventManager.Models
{
	public class MyEventListItem
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public int Capacity { get; set; }
		public decimal Price { get; set; }
		public string? ImageUrl { get; set; }
		public string Status { get; set; } = string.Empty;
		public int RegisteredCount { get; set; }
	}
}