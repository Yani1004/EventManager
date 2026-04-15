using EventManager.Data;
using EventManager.Models;
using EventManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Services
{
	public class AdminService : IAdminService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _db;

		public AdminService(
			UserManager<ApplicationUser> userManager,
			ApplicationDbContext db)
		{
			_userManager = userManager;
			_db = db;
		}

		public async Task<AdminDashboardData> GetDashboardDataAsync()
		{
			var result = new AdminDashboardData();

			result.TotalUsers = await _userManager.Users.CountAsync();

			result.PendingApprovals = await _userManager.Users
				.CountAsync(u => u.RequestedRole == "EventManager" && !u.IsApproved);

			result.TotalEvents = await _db.Events.CountAsync();

			var users = await _userManager.Users.ToListAsync();

			result.TotalEventManagers = 0;
			foreach (var user in users)
			{
				if (await _userManager.IsInRoleAsync(user, "EventManager"))
				{
					result.TotalEventManagers++;
				}
			}

			result.RecentPendingUsers = await _userManager.Users
				.Where(u => u.RequestedRole == "EventManager" && !u.IsApproved)
				.OrderBy(u => u.FirstName)
				.ThenBy(u => u.LastName)
				.Take(5)
				.ToListAsync();

			return result;
		}

		public async Task<List<PendingEventManagerListItem>> GetPendingEventManagerRequestsAsync()
		{
			var users = await _userManager.Users
				.Where(u => u.RequestedRole == "EventManager" && !u.IsApproved)
				.OrderBy(u => u.FirstName)
				.ThenBy(u => u.LastName)
				.ToListAsync();

			return users.Select(user => new PendingEventManagerListItem
			{
				Id = user.Id,
				Email = user.Email ?? string.Empty,
				FirstName = user.FirstName,
				LastName = user.LastName,
				RequestedRole = user.RequestedRole,
				IsApproved = user.IsApproved
			}).ToList();
		}

		public async Task<ServiceResult> ApproveEventManagerAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return ServiceResult.Fail("User not found.");
			}

			user.IsApproved = true;
			user.RequestedRole = "EventManager";

			var updateResult = await _userManager.UpdateAsync(user);

			if (!updateResult.Succeeded)
			{
				return ServiceResult.Fail(string.Join(" ", updateResult.Errors.Select(e => e.Description)));
			}

			if (!await _userManager.IsInRoleAsync(user, "EventManager"))
			{
				var addRoleResult = await _userManager.AddToRoleAsync(user, "EventManager");
				if (!addRoleResult.Succeeded)
				{
					return ServiceResult.Fail(string.Join(" ", addRoleResult.Errors.Select(e => e.Description)));
				}
			}

			if (await _userManager.IsInRoleAsync(user, "Attendee"))
			{
				var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, "Attendee");
				if (!removeRoleResult.Succeeded)
				{
					return ServiceResult.Fail(string.Join(" ", removeRoleResult.Errors.Select(e => e.Description)));
				}
			}

			return ServiceResult.Ok($"User {user.Email} approved as Event Manager.");
		}

		public async Task<ServiceResult> RejectEventManagerAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return ServiceResult.Fail("User not found.");
			}

			user.RequestedRole = "Rejected";
			user.IsApproved = false;

			var updateResult = await _userManager.UpdateAsync(user);

			if (!updateResult.Succeeded)
			{
				return ServiceResult.Fail(string.Join(" ", updateResult.Errors.Select(e => e.Description)));
			}

			if (!await _userManager.IsInRoleAsync(user, "Attendee"))
			{
				var addRoleResult = await _userManager.AddToRoleAsync(user, "Attendee");
				if (!addRoleResult.Succeeded)
				{
					return ServiceResult.Fail(string.Join(" ", addRoleResult.Errors.Select(e => e.Description)));
				}
			}

			if (await _userManager.IsInRoleAsync(user, "EventManager"))
			{
				var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, "EventManager");
				if (!removeRoleResult.Succeeded)
				{
					return ServiceResult.Fail(string.Join(" ", removeRoleResult.Errors.Select(e => e.Description)));
				}
			}

			return ServiceResult.Ok($"User {user.Email} request rejected.");
		}

		public async Task<List<UserListItem>> GetAllUsersAsync()
		{
			var allUsers = await _userManager.Users
				.OrderBy(u => u.FirstName)
				.ThenBy(u => u.LastName)
				.ToListAsync();

			var result = new List<UserListItem>();

			foreach (var user in allUsers)
			{
				var roles = await _userManager.GetRolesAsync(user);

				result.Add(new UserListItem
				{
					Id = user.Id,
					FullName = $"{user.FirstName} {user.LastName}".Trim(),
					Email = user.Email ?? string.Empty,
					RequestedRole = user.RequestedRole,
					IsApproved = user.IsApproved,
					CurrentRole = roles.FirstOrDefault() ?? "No Role"
				});
			}

			return result;
		}

		public async Task<List<AdminEventListItem>> GetAllEventsForAdminAsync()
		{
			var events = await _db.Events
				.OrderBy(e => e.Date)
				.ToListAsync();

			var creatorIds = events
				.Where(e => !string.IsNullOrWhiteSpace(e.CreatorId))
				.Select(e => e.CreatorId)
				.Distinct()
				.ToList();

			var creatorNames = new Dictionary<string, string>();

			foreach (var creatorId in creatorIds)
			{
				var user = await _userManager.FindByIdAsync(creatorId!);

				if (user != null)
				{
					var fullName = $"{user.FirstName} {user.LastName}".Trim();
					creatorNames[creatorId!] = string.IsNullOrWhiteSpace(fullName)
						? user.Email ?? "Unknown"
						: fullName;
				}
				else
				{
					creatorNames[creatorId!] = "Unknown";
				}
			}

			return events.Select(ev => new AdminEventListItem
			{
				Id = ev.Id,
				Title = ev.Title,
				Description = ev.Description,
				Date = ev.Date,
				Location = ev.Location,
				Category = ev.Category,
				Status = ev.Status,
				AdminNote = ev.AdminNote,
				CreatorId = ev.CreatorId,
				CreatorName = !string.IsNullOrWhiteSpace(ev.CreatorId) && creatorNames.TryGetValue(ev.CreatorId, out var creatorName)
					? creatorName
					: "Unknown",
				Price = ev.Price,
				ImageUrl = ev.ImageUrl,
				Capacity = ev.Capacity
			}).ToList();
		}

		public async Task<ServiceResult> FlagEventAsync(int eventId, string adminNote)
		{
			var ev = await _db.Events.FindAsync(eventId);

			if (ev == null)
			{
				return ServiceResult.Fail("Event not found.");
			}

			if (string.IsNullOrWhiteSpace(adminNote))
			{
				return ServiceResult.Fail("Please write an admin note before flagging the event.");
			}

			ev.Status = "Flagged";
			ev.AdminNote = adminNote.Trim();

			await _db.SaveChangesAsync();

			return ServiceResult.Ok($"Event \"{ev.Title}\" was flagged successfully.");
		}

		public async Task<ServiceResult> UpdateFlaggedNoteAsync(int eventId, string adminNote)
		{
			var ev = await _db.Events.FindAsync(eventId);

			if (ev == null)
			{
				return ServiceResult.Fail("Event not found.");
			}

			if (ev.Status != "Flagged")
			{
				return ServiceResult.Fail("Only flagged events can update the saved admin note.");
			}

			if (string.IsNullOrWhiteSpace(adminNote))
			{
				return ServiceResult.Fail("Please write an admin note.");
			}

			ev.AdminNote = adminNote.Trim();

			await _db.SaveChangesAsync();

			return ServiceResult.Ok($"Admin note updated for \"{ev.Title}\".");
		}

		public async Task<ServiceResult> UpdateEventStatusAsync(int eventId, string newStatus)
		{
			var ev = await _db.Events.FindAsync(eventId);

			if (ev == null)
			{
				return ServiceResult.Fail("Event not found.");
			}

			ev.Status = newStatus;

			if (newStatus != "Flagged")
			{
				ev.AdminNote = null;
			}

			await _db.SaveChangesAsync();

			var label = newStatus == "PendingReview" ? "Pending Review" : newStatus;
			return ServiceResult.Ok($"Event \"{ev.Title}\" updated to {label}.");
		}
	}
}