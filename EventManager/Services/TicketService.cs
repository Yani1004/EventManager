using EventManager.Data;
using EventManager.Models;
using EventManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Services
{
	public class TicketService : ITicketService
	{
		private readonly ApplicationDbContext _db;

		public TicketService(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<Ticket> IssueTicketAsync(int registrationId)
		{
			var existingTicket = await _db.Tickets
				.FirstOrDefaultAsync(t => t.RegistrationId == registrationId);

			if (existingTicket != null)
			{
				return existingTicket;
			}

			var ticket = new Ticket
			{
				RegistrationId = registrationId,
				TicketNumber = GenerateTicketNumber(),
				IssuedAt = DateTime.UtcNow,
				VerificationCode = GenerateVerificationCode()
			};

			_db.Tickets.Add(ticket);
			await _db.SaveChangesAsync();

			return ticket;
		}

		public async Task<TicketDetailsDto?> GetTicketDetailsAsync(int registrationId, string userId)
		{
			return await _db.Registrations
				.Where(r => r.Id == registrationId && r.UserId == userId)
				.Select(r => new TicketDetailsDto
					{
						RegistrationId = r.Id,
						EventId = r.EventId,
						EventTitle = r.Event.Title,
					EventLocation = r.Event.Location,
					EventDate = r.Event.Date,
					Price = r.Event.Price,
					RegisteredAt = r.RegisteredAt,
					CategoryName = r.Event.EventCategory.Name,
					TicketNumber = r.Ticket != null ? r.Ticket.TicketNumber : null,
					VerificationCode = r.Ticket != null ? r.Ticket.VerificationCode : null,
						PaymentMethod = r.Payment != null ? r.Payment.PaymentMethod : null,
						PaymentStatus = r.Payment != null ? r.Payment.PaymentStatus : null,
						TransactionReference = r.Payment != null ? r.Payment.TransactionReference : null
					})
					.FirstOrDefaultAsync();
			}

		private static string GenerateTicketNumber()
		{
			return $"TKT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..26];
		}

		private static string GenerateVerificationCode()
		{
			return Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
		}
	}
}
