using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskMaster.Models;
using TaskMaster.Services.Interfaces;
using TaskMaster.DTOs.Tasks;

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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _taskService.CreateAsync(task, userId));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _taskService.GetAllAsync());
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
    return Ok(await _taskService.FilterAsync(
        status, priority, dueBefore, page, pageSize));
    }
    }

