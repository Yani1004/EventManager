using EventManager.Data;
using EventManager.Models;
using EventManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Services
{
	public class RegistrationService : IRegistrationService
	{
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;

		public RegistrationService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
		{
			_db = db;
			_userManager = userManager;
		}

		public async Task<int> GetRegisteredCountAsync(int eventId)
		{
			return await _db.Registrations.CountAsync(r => r.EventId == eventId);
		}

		public async Task<Registration?> GetUserRegistrationAsync(int eventId, string userId)
		{
			return await _db.Registrations
				.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
		}

		public async Task<ServiceResult> RegisterForEventAsync(int eventId, string userId)
		{
			var eventItem = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId);

			if (eventItem == null)
			{
				return ServiceResult.Fail("Event not found.");
			}

			if (eventItem.Status != "Active")
			{
				return ServiceResult.Fail("This event is currently not available for registration.");
			}

			var alreadyRegistered = await _db.Registrations
				.AnyAsync(r => r.EventId == eventId && r.UserId == userId);

			if (alreadyRegistered)
			{
				return ServiceResult.Fail("You are already registered for this event.");
			}

			var currentCount = await _db.Registrations.CountAsync(r => r.EventId == eventId);

			if (currentCount >= eventItem.Capacity)
			{
				return ServiceResult.Fail("This event is already full.");
			}

			var registration = new Registration
			{
				EventId = eventId,
				UserId = userId,
				RegisteredAt = DateTime.UtcNow
			};

			_db.Registrations.Add(registration);
			await _db.SaveChangesAsync();

			return ServiceResult.Ok("You successfully registered for the event.");
		}

		public async Task<ServiceResult> CancelRegistrationAsync(int registrationId, string userId)
		{
			var registration = await _db.Registrations
				.FirstOrDefaultAsync(r => r.Id == registrationId && r.UserId == userId);

			if (registration == null)
			{
				return ServiceResult.Fail("Registration not found.");
			}

			_db.Registrations.Remove(registration);
			await _db.SaveChangesAsync();

			return ServiceResult.Ok("Registration cancelled successfully.");
		}

		public async Task<List<EventRegistrationListItem>> GetEventRegistrationsForOwnerAsync(int eventId, string ownerUserId)
		{
			var eventItem = await _db.Events
				.FirstOrDefaultAsync(e => e.Id == eventId && e.CreatorId == ownerUserId);

			if (eventItem == null)
			{
				return new List<EventRegistrationListItem>();
			}

			var eventRegistrations = await _db.Registrations
				.Where(r => r.EventId == eventId)
				.OrderByDescending(r => r.RegisteredAt)
				.ToListAsync();

			var result = new List<EventRegistrationListItem>();

			foreach (var registration in eventRegistrations)
			{
				var registeredUser = await _userManager.FindByIdAsync(registration.UserId);

				if (registeredUser != null)
				{
					result.Add(new EventRegistrationListItem
					{
						UserName = registeredUser.UserName ?? string.Empty,
						FullName = $"{registeredUser.FirstName} {registeredUser.LastName}".Trim(),
						Email = registeredUser.Email ?? string.Empty,
						RegisteredAt = registration.RegisteredAt
					});
				}
			}

			return result;
		}
	}
}