using EventManager.Models;

namespace EventManager.Services.Interfaces
{
	public interface IEventService
	{
		Task<ServiceResult> CreateEventAsync(Event eventModel, DateOnly eventDateOnly, TimeOnly selectedTime, string userId);
		Task<Event?> GetEventByIdAsync(int eventId);
		Task<Event?> GetOwnedEventByIdAsync(int eventId, string userId);
		Task<List<MyEventListItem>> GetMyEventsAsync(string userId);
		Task<List<Event>> GetActiveEventsAsync();
		Task<ServiceResult> UpdateEventAsync(Event eventModel, DateOnly eventDateOnly, TimeOnly selectedTime, string userId);
		Task<ServiceResult> DeleteEventAsync(int eventId, string userId);
	}
}