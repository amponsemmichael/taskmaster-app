using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.DTOs.Tasks;
using TaskMaster.Extensions;
using TaskMaster.Models;
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
    public async Task<IActionResult> Create(TaskItem task)
    {
        var userId = User.GetUserId();
        var createdTask = await _taskService.CreateAsync(task, userId);
        return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound(new { message = $"Task with ID {id} not found" });

        return Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        var userId = User.GetUserId();
        var updatedTask = await _taskService.UpdateAsync(id, dto, userId);
        return Ok(updatedTask);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        await _taskService.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Assign(Guid id, AssignedTaskDto dto)
    {
        await _taskService.AssignAsync(id, dto.UserId);
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
            status, priority, dueBefore, page, pageSize);
        return Ok(tasks);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] bool myTasks = false)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search term is required" });

        var userId = myTasks ? User.GetUserId() : (Guid?)null;
        var tasks = await _taskService.SearchAsync(q, userId);
        return Ok(tasks);
    }
}