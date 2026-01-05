using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Services;

namespace TaskMaster.Controllers;

[ApiController]
[Route("api/tasks/{taskId}/activity")]
[Authorize]
public class TaskActivityController : ControllerBase
{
    private readonly ActivityLogService _service;

    public TaskActivityController(ActivityLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetActivity(Guid taskId)
    {
        var logs = await _service.GetByTaskIdAsync(taskId);
        return Ok(logs);
    }
}
