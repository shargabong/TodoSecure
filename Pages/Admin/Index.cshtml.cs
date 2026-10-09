using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoSecure.Models;

namespace TodoSecure.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(UserManager<AppUser> userManager) => _userManager = userManager;

    public List<UserWithRoles> Users { get; set; } = new();

    public class UserWithRoles
    {
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public async Task OnGetAsync()
    {
        foreach (var u in _userManager.Users.ToList())
        {
            Users.Add(new UserWithRoles
            {
                Email = u.Email ?? "",
                FullName = u.FullName,
                Roles = await _userManager.GetRolesAsync(u)
            });
        }
    }
}