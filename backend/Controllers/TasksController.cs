using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.DTOs.Tasks;
using TaskMaster.Extensions;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var userId = User.GetUserId();
        var createdTask = await _taskService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskService.GetAllAsync(
            User.IsInRole("ADMIN") ? null : User.GetUserId());
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(
            id,
            User.IsInRole("ADMIN") ? null : User.GetUserId());
        if (task == null)
            return NotFound(new { message = $"Task with ID {id} not found" });

        return Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        var userId = User.GetUserId();
        var updatedTask = await _taskService.UpdateAsync(
            id, dto, userId, User.IsInRole("ADMIN"));
        return Ok(updatedTask);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        await _taskService.DeleteAsync(id, userId, User.IsInRole("ADMIN"));
        return NoContent();
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Assign(Guid id, AssignedTaskDto dto)
    {
        if (dto.UserId == Guid.Empty)
            return BadRequest(new { message = "UserId must be a non-empty GUID" });

        var assignedByUserId = User.GetUserId();
        await _taskService.AssignAsync(id, dto.UserId, assignedByUserId);
        return NoContent();
    }

    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] DateTime? dueBefore,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var tasks = await _taskService.FilterAsync(
            status,
            priority,
            dueBefore,
            page,
            pageSize,
            User.IsInRole("ADMIN") ? null : User.GetUserId());
        return Ok(tasks);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] bool myTasks = false)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search term is required" });

        if (q.Length > 200)
            return BadRequest(new { message = "Search term must not exceed 200 characters" });

        var isAdmin = User.IsInRole("ADMIN");
        Guid? userId = isAdmin && !myTasks ? null : User.GetUserId();
        var tasks = await _taskService.SearchAsync(q, userId);
        return Ok(tasks);
    }
}