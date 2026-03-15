namespace EventManager.Models
{
    public class PendingEventManagerViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string RequestedRole { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}