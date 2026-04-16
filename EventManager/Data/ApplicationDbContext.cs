using EventManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Event> Events { get; set; }
		public DbSet<Registration> Registrations { get; set; }

		public DbSet<EventCategory> EventCategories { get; set; }
		public DbSet<EventStatus> EventStatuses { get; set; }
		public DbSet<RegistrationStatus> RegistrationStatuses { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<EmailLog> EmailLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.HasDefaultSchema(DatabaseSchema.Name);

			base.OnModelCreating(builder);

			builder.Entity<EventCategory>()
				.HasIndex(x => x.Name)
				.IsUnique();

			builder.Entity<EventStatus>()
				.HasIndex(x => x.Name)
				.IsUnique();

			builder.Entity<RegistrationStatus>()
				.HasIndex(x => x.Name)
				.IsUnique();

			builder.Entity<Ticket>()
				.HasIndex(x => x.TicketNumber)
				.IsUnique();

			builder.Entity<Payment>()
				.HasIndex(x => x.RegistrationId)
				.IsUnique();

			builder.Entity<Payment>()
				.HasIndex(x => x.TransactionReference)
				.IsUnique();

			builder.Entity<Event>()
				.HasOne(e => e.EventCategory)
				.WithMany(c => c.Events)
				.HasForeignKey(e => e.EventCategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Event>()
				.HasOne(e => e.EventStatus)
				.WithMany(s => s.Events)
				.HasForeignKey(e => e.EventStatusId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Registration>()
				.HasOne(r => r.Event)
				.WithMany(e => e.Registrations)
				.HasForeignKey(r => r.EventId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Registration>()
				.HasOne(r => r.User)
				.WithMany()
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Registration>()
				.HasOne(r => r.RegistrationStatus)
				.WithMany(s => s.Registrations)
				.HasForeignKey(r => r.RegistrationStatusId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Registration>()
				.HasIndex(r => new { r.EventId, r.UserId })
				.IsUnique();

			builder.Entity<Ticket>()
				.HasOne(t => t.Registration)
				.WithOne(r => r.Ticket)
				.HasForeignKey<Ticket>(t => t.RegistrationId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Payment>()
				.HasOne(p => p.Registration)
				.WithOne(r => r.Payment)
				.HasForeignKey<Payment>(p => p.RegistrationId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<EventCategory>().HasData(
				new EventCategory { Id = 1, Name = "Conference" },
				new EventCategory { Id = 2, Name = "Workshop" },
				new EventCategory { Id = 3, Name = "Concert" },
				new EventCategory { Id = 4, Name = "Sports" },
				new EventCategory { Id = 5, Name = "Networking" },
				new EventCategory { Id = 6, Name = "Festival" },
				new EventCategory { Id = 7, Name = "Seminar" },
				new EventCategory { Id = 8, Name = "Other" }
			);

			builder.Entity<EventStatus>().HasData(
				new EventStatus { Id = 1, Name = "Draft" },
				new EventStatus { Id = 2, Name = "Active" },
				new EventStatus { Id = 3, Name = "Cancelled" },
				new EventStatus { Id = 4, Name = "Archived" }
			);

			builder.Entity<RegistrationStatus>().HasData(
				new RegistrationStatus { Id = 1, Name = "Registered" },
				new RegistrationStatus { Id = 2, Name = "Cancelled" },
				new RegistrationStatus { Id = 3, Name = "Attended" },
				new RegistrationStatus { Id = 4, Name = "NoShow" }
			);
		}
	}
}
