using EventManager.Models;

namespace EventManager.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardData> GetDashboardDataAsync();
        Task<List<PendingEventManagerListItem>> GetPendingEventManagerRequestsAsync();
        Task<ServiceResult> ApproveEventManagerAsync(string userId);
        Task<ServiceResult> RejectEventManagerAsync(string userId);
        Task<List<UserListItem>> GetAllUsersAsync();
        Task<List<AdminEventListItem>> GetAllEventsForAdminAsync();
        Task<ServiceResult> FlagEventAsync(int eventId, string adminNote);
        Task<ServiceResult> UpdateFlaggedNoteAsync(int eventId, string adminNote);
		Task<ServiceResult> UpdateEventStatusAsync(int eventId, int newStatusId);
	}
}