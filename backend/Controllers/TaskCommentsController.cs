using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskMaster.DTOs;
using TaskMaster.Extensions;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Controllers;

[ApiController]
[Route("api/tasks/{taskId}/comments")]
[Authorize]
public class TaskCommentsController : ControllerBase
{
    private readonly ITaskCommentService _service;

    public TaskCommentsController(ITaskCommentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(
        Guid taskId,
        CreateCommentDto dto)
    {
        var userId = User.GetUserId();
        await _service.AddCommentAsync(taskId, userId, User.IsInRole("ADMIN"), dto);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(Guid taskId)
    {
        var comments = await _service.GetCommentsAsync(
            taskId, User.GetUserId(), User.IsInRole("ADMIN"));
        return Ok(comments);
    }
}
