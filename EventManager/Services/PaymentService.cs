using EventManager.Data;
using EventManager.Models;
using EventManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly ApplicationDbContext _db;

		public PaymentService(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<Payment> CreatePaymentAsync(int registrationId, decimal amount, RegistrationCheckoutRequest checkoutRequest)
		{
			var existingPayment = await _db.Payments
				.FirstOrDefaultAsync(p => p.RegistrationId == registrationId);

			if (existingPayment != null)
			{
				return existingPayment;
			}

			var paymentMethod = amount == 0
				? "Free"
				: string.IsNullOrWhiteSpace(checkoutRequest.PaymentMethod) ? "Card" : checkoutRequest.PaymentMethod.Trim();

			var cardNumber = new string((checkoutRequest.CardNumber ?? string.Empty).Where(char.IsDigit).ToArray());

			var payment = new Payment
			{
				RegistrationId = registrationId,
				Amount = amount,
				PaymentMethod = paymentMethod,
				PaymentStatus = "Completed",
				CardholderName = paymentMethod == "Card" ? checkoutRequest.CardholderName?.Trim() : null,
				CardLast4 = paymentMethod == "Card" && cardNumber.Length >= 4 ? cardNumber[^4..] : null,
				CardBrand = paymentMethod == "Card" ? DetectCardBrand(cardNumber) : null,
				TransactionReference = GenerateTransactionReference(),
				PaidAt = DateTime.UtcNow
			};

			_db.Payments.Add(payment);
			await _db.SaveChangesAsync();

			return payment;
		}

		private static string GenerateTransactionReference()
		{
			return $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..32];
		}

		private static string DetectCardBrand(string cardNumber)
		{
			if (string.IsNullOrWhiteSpace(cardNumber))
			{
				return "Unknown";
			}

			return cardNumber[0] switch
			{
				'4' => "Visa",
				'5' => "Mastercard",
				'3' => "American Express",
				_ => "Card"
			};
		}
	}
}
