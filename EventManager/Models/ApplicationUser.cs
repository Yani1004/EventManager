using Microsoft.AspNetCore.Identity;

namespace EventManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string RequestedRole { get; set; } = "Attendee";
        public bool IsApproved { get; set; } = true;
    }
}