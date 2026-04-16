namespace EventManager.Models
{
	public class RegistrationCheckoutRequest
	{
		public string PaymentMethod { get; set; } = "Card";
		public string? CardholderName { get; set; }
		public string? CardNumber { get; set; }
		public string? ExpiryDate { get; set; }
		public string? Cvv { get; set; }
	}
}
