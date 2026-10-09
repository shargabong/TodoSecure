using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IAuthorizationService _auth;

    public EditModel(AppDbContext db, IAuthorizationService auth)
    {
        _db = db;
        _auth = auth;
    }

    [BindProperty]
    public TaskItem Task { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var result = await _auth.AuthorizeAsync(User, task, "CanEditTask");
        if (!result.Succeeded) return Forbid();

        Task = task;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var existing = await _db.Tasks.FindAsync(Task.Id);
        if (existing == null) return NotFound();

        var result = await _auth.AuthorizeAsync(User, existing, "CanEditTask");
        if (!result.Succeeded) return Forbid();

        existing.Title = Task.Title;
        existing.Description = Task.Description;
        existing.IsCompleted = Task.IsCompleted;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}