using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty]
    public TaskItem Task { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        Task.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        Task.CreatedAt = DateTime.UtcNow;

        _db.Tasks.Add(Task);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}