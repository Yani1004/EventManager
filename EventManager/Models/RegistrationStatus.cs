namespace EventManager.Models
{
	public class RegistrationStatus
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;

		public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
	}
}