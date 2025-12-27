[ApiController]
[Route("api/tasks/{taskId}/comments")]
[Authorize]
public class TaskCommentsController : ControllerBase
{
    private readonly TaskCommentService _service;

    public TaskCommentsController(TaskCommentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(
        Guid taskId,
        CreateCommentDto dto)
    {
        var userId = User.GetUserId(); // your existing extension
        await _service.AddCommentAsync(taskId, userId, dto);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(Guid taskId)
    {
        var comments = await _service.GetCommentsAsync(taskId);
        return Ok(comments);
    }
}
