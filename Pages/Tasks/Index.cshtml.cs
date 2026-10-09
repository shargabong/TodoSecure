using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoSecure.Data;
using TodoSecure.Models;

namespace TodoSecure.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IAuthorizationService _auth;

    public IndexModel(AppDbContext db, IAuthorizationService auth)
    {
        _db = db;
        _auth = auth;
    }

    public List<TaskItem> Tasks { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tasks = await _db.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var result = await _auth.AuthorizeAsync(User, task, "CanEditTask");
        if (!result.Succeeded) return Forbid();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var result = await _auth.AuthorizeAsync(User, task, "CanEditTask");
        if (!result.Succeeded) return Forbid();

        task.IsCompleted = !task.IsCompleted;
        await _db.SaveChangesAsync();
        return new JsonResult(new { success = true, isCompleted = task.IsCompleted });
    }
}