using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoSecure.Data;
using TodoSecure.DTOs;
using TodoSecure.Models;

namespace TodoSecure.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class TasksApiController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksApiController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var query = _db.Tasks.AsQueryable();
        if (!isAdmin)
            query = query.Where(t => t.UserId == userId);

        var tasks = await query.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            CreatedAt = t.CreatedAt,
            UserId = t.UserId
        }).ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        if (!isAdmin && task.UserId != userId) return Forbid();

        return Ok(new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            UserId = task.UserId
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId, // ⚠️ берём из токена, не из body
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = task.Id }, task.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        if (!isAdmin && task.UserId != userId) return Forbid();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        if (!isAdmin && task.UserId != userId) return Forbid();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}