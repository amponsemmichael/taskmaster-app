using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Controllers;

[ApiController]
[Route("api/tasks/{taskId}/activity")]
[Authorize]
public class TaskActivityController : ControllerBase
{
    private readonly IActivityLogService _service;

    public TaskActivityController(IActivityLogService service)
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
