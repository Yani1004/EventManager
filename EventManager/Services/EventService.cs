using EventManager.Data;
using EventManager.Models;
using EventManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Services
{
	public class EventService : IEventService
	{
		private readonly ApplicationDbContext _db;

		public EventService(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<ServiceResult> CreateEventAsync(Event eventModel, DateOnly eventDateOnly, TimeOnly selectedTime, string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				return ServiceResult.Fail("User not found.");
			}

			var localDateTime = eventDateOnly.ToDateTime(selectedTime);
			var utcDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local).ToUniversalTime();

			var pendingReviewStatusId = await _db.EventStatuses
				.Where(s => s.Name == "PendingReview")
				.Select(s => s.Id)
				.FirstOrDefaultAsync();

			if (pendingReviewStatusId == 0)
			{
				return ServiceResult.Fail("Pending review status not found.");
			}

			eventModel.Date = utcDateTime;
			eventModel.CreatedAt = DateTime.UtcNow;
			eventModel.CreatorId = userId;
			eventModel.EventStatusId = pendingReviewStatusId;

			if (string.IsNullOrWhiteSpace(eventModel.AdminNote))
			{
				eventModel.AdminNote = null;
			}

			_db.Events.Add(eventModel);
			await _db.SaveChangesAsync();

			return ServiceResult.Ok("Event created successfully.");
		}

		public async Task<Event?> GetEventByIdAsync(int eventId)
		{
			return await _db.Events
				.Include(e => e.EventCategory)
				.Include(e => e.EventStatus)
				.FirstOrDefaultAsync(e => e.Id == eventId);
		}

		public async Task<Event?> GetOwnedEventByIdAsync(int eventId, string userId)
		{
			return await _db.Events
				.Include(e => e.EventCategory)
				.Include(e => e.EventStatus)
				.FirstOrDefaultAsync(e => e.Id == eventId && e.CreatorId == userId);
		}

		public async Task<List<MyEventListItem>> GetMyEventsAsync(string userId)
		{
			return await _db.Events
				.Where(e => e.CreatorId == userId)
				.OrderByDescending(e => e.Date)
				.Select(e => new MyEventListItem
				{
					Id = e.Id,
					Title = e.Title,
					CategoryId = e.EventCategoryId,
					CategoryName = e.EventCategory.Name,
					Location = e.Location,
					Date = e.Date,
					Capacity = e.Capacity,
					Price = e.Price,
					ImageUrl = e.ImageUrl,
					StatusId = e.EventStatusId,
					StatusName = e.EventStatus.Name,
					RegisteredCount = e.Registrations.Count
				})
				.ToListAsync();
		}

		public async Task<List<Event>> GetActiveEventsAsync()
		{
			return await _db.Events
				.Include(e => e.EventCategory)
				.Include(e => e.EventStatus)
				.Where(e => e.EventStatus.Name == "Active")
				.OrderBy(e => e.Date)
				.ToListAsync();
		}

		public async Task<ServiceResult> UpdateEventAsync(Event eventModel, DateOnly eventDateOnly, TimeOnly selectedTime, string userId)
		{
			var existingEvent = await _db.Events
				.Include(e => e.EventStatus)
				.FirstOrDefaultAsync(e => e.Id == eventModel.Id && e.CreatorId == userId);

			if (existingEvent == null)
			{
				return ServiceResult.Fail("Event not found or access denied.");
			}

			var localDateTime = eventDateOnly.ToDateTime(selectedTime);
			var utcDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local).ToUniversalTime();

			existingEvent.Title = eventModel.Title;
			existingEvent.Description = eventModel.Description;
			existingEvent.Location = eventModel.Location;
			existingEvent.Date = utcDateTime;
			existingEvent.Capacity = eventModel.Capacity;
			existingEvent.Price = eventModel.Price;
			existingEvent.ImageUrl = eventModel.ImageUrl;
			existingEvent.EventCategoryId = eventModel.EventCategoryId;

			if (existingEvent.EventStatus.Name == "Flagged")
			{
				var pendingReviewStatusId = await _db.EventStatuses
					.Where(s => s.Name == "PendingReview")
					.Select(s => s.Id)
					.FirstOrDefaultAsync();

				if (pendingReviewStatusId == 0)
				{
					return ServiceResult.Fail("Pending review status not found.");
				}

				existingEvent.EventStatusId = pendingReviewStatusId;
			}

			await _db.SaveChangesAsync();

			return ServiceResult.Ok("Event updated successfully.");
		}

		public async Task<ServiceResult> DeleteEventAsync(int eventId, string userId)
		{
			var eventItem = await _db.Events
				.FirstOrDefaultAsync(e => e.Id == eventId && e.CreatorId == userId);

			if (eventItem == null)
			{
				return ServiceResult.Fail("Event not found or access denied.");
			}

			_db.Events.Remove(eventItem);
			await _db.SaveChangesAsync();

			return ServiceResult.Ok("Event deleted successfully.");
		}
	}
}