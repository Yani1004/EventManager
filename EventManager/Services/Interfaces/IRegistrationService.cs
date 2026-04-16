using EventManager.Models;

namespace EventManager.Services.Interfaces
{
	public interface IRegistrationService
	{
		Task<int> GetRegisteredCountAsync(int eventId);
		Task<Registration?> GetUserRegistrationAsync(int eventId, string userId);
		Task<List<MyRegistrationListItem>> GetUserRegistrationsAsync(string userId);
		Task<TicketDetailsDto?> GetTicketDetailsAsync(int registrationId, string userId);
		Task<ServiceResult> CompleteRegistrationAsync(int eventId, string userId);
		Task<ServiceResult> CancelRegistrationAsync(int registrationId, string userId);
		Task<List<EventRegistrationListItem>> GetEventRegistrationsForOwnerAsync(int eventId, string ownerUserId);
	}
}
