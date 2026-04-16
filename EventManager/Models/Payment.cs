using System.ComponentModel.DataAnnotations;

namespace EventManager.Models
{
	public class Payment
	{
		public int Id { get; set; }

		[Required]
		public int RegistrationId { get; set; }
		public Registration Registration { get; set; } = null!;

		[Range(0, 100000)]
		public decimal Amount { get; set; }

		[Required]
		[StringLength(50)]
		public string PaymentMethod { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string PaymentStatus { get; set; } = string.Empty;

		[StringLength(150)]
		public string? CardholderName { get; set; }

		[StringLength(4)]
		public string? CardLast4 { get; set; }

		[StringLength(30)]
		public string? CardBrand { get; set; }

		[Required]
		[StringLength(50)]
		public string Currency { get; set; } = "BGN";

		[Required]
		[StringLength(100)]
		public string TransactionReference { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? PaidAt { get; set; }
	}
}
