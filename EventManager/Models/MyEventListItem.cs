namespace EventManager.Models
{
	public class MyEventListItem
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public int CategoryId { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public int StatusId { get; set; }
		public string StatusName { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public int Capacity { get; set; }
		public decimal Price { get; set; }
		public string? ImageUrl { get; set; }
		public int RegisteredCount { get; set; }
	}
}