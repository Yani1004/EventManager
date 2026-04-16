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
		private readonly IPaymentService _paymentService;
		private readonly ITicketService _ticketService;

		public RegistrationService(
			ApplicationDbContext db,
			UserManager<ApplicationUser> userManager,
			IPaymentService paymentService,
			ITicketService ticketService)
		{
			_db = db;
			_userManager = userManager;
			_paymentService = paymentService;
			_ticketService = ticketService;
		}

		public async Task<int> GetRegisteredCountAsync(int eventId)
		{
			return await _db.Registrations.CountAsync(r => r.EventId == eventId);
		}

		public async Task<Registration?> GetUserRegistrationAsync(int eventId, string userId)
		{
			return await _db.Registrations
				.Include(r => r.Ticket)
				.Include(r => r.Payment)
				.Include(r => r.Event)
					.ThenInclude(e => e.EventStatus)
				.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
		}

		public async Task<List<MyRegistrationListItem>> GetUserRegistrationsAsync(string userId)
		{
			return await _db.Registrations
				.Where(r => r.UserId == userId)
				.OrderByDescending(r => r.RegisteredAt)
				.Select(r => new MyRegistrationListItem
				{
					RegistrationId = r.Id,
					EventId = r.EventId,
					EventTitle = r.Event.Title,
					EventImageUrl = r.Event.ImageUrl,
					EventLocation = r.Event.Location,
					EventDate = r.Event.Date,
						RegisteredAt = r.RegisteredAt,
						Price = r.Event.Price,
						CategoryName = r.Event.EventCategory.Name,
						TicketNumber = r.Ticket != null ? r.Ticket.TicketNumber : null,
						VerificationCode = r.Ticket != null ? r.Ticket.VerificationCode : null,
						PaymentMethod = r.Payment != null ? r.Payment.PaymentMethod : null,
						PaymentStatus = r.Payment != null ? r.Payment.PaymentStatus : null
					})
				.ToListAsync();
		}

		public async Task<ServiceResult> CompleteRegistrationAsync(int eventId, string userId, RegistrationCheckoutRequest checkoutRequest)
		{
			var eventItem = await _db.Events
				.Include(e => e.EventStatus)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			if (eventItem == null)
			{
				return ServiceResult.Fail("Event not found.");
			}

			if (eventItem.EventStatus.Name != "Active")
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

			var registeredStatusId = await _db.RegistrationStatuses
				.Where(s => s.Name == "Registered")
				.Select(s => s.Id)
				.FirstOrDefaultAsync();

			if (registeredStatusId == 0)
			{
				return ServiceResult.Fail("Registration status not found.");
			}

			if (eventItem.Price > 0 && string.IsNullOrWhiteSpace(checkoutRequest.PaymentMethod))
			{
				return ServiceResult.Fail("Payment method is required.");
			}

			var registration = new Registration
			{
				EventId = eventId,
				UserId = userId,
				RegisteredAt = DateTime.UtcNow,
				RegistrationStatusId = registeredStatusId
			};

			_db.Registrations.Add(registration);
			await _db.SaveChangesAsync();

			await _paymentService.CreatePaymentAsync(registration.Id, eventItem.Price, checkoutRequest);
			await _ticketService.IssueTicketAsync(registration.Id);

			return ServiceResult.Ok(eventItem.Price > 0
				? "Payment successful. Your ticket is confirmed."
				: "Registration confirmed. Your ticket is ready.");
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
