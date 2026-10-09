using Microsoft.AspNetCore.Identity;

namespace TodoSecure.Models;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}