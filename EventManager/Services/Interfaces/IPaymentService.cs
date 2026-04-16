using EventManager.Models;

namespace EventManager.Services.Interfaces
{
	public interface IPaymentService
	{
		Task<Payment> CreatePaymentAsync(int registrationId, decimal amount, RegistrationCheckoutRequest checkoutRequest);
	}
}
