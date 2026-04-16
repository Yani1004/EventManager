using EventManager.Models;

namespace EventManager.Services.Interfaces
{
	public interface ITicketService
	{
		Task<Ticket> IssueTicketAsync(int registrationId);
		Task<TicketDetailsDto?> GetTicketDetailsAsync(int registrationId, string userId);
	}
}
