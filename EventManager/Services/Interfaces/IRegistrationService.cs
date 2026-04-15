using EventManager.Models;

namespace EventManager.Services.Interfaces
{
	public interface IRegistrationService
	{
		Task<int> GetRegisteredCountAsync(int eventId);
		Task<Registration?> GetUserRegistrationAsync(int eventId, string userId);
		Task<ServiceResult> RegisterForEventAsync(int eventId, string userId);
		Task<ServiceResult> CancelRegistrationAsync(int registrationId, string userId);
		Task<List<EventRegistrationListItem>> GetEventRegistrationsForOwnerAsync(int eventId, string ownerUserId);
	}
}