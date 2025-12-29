namespace TaskMaster.Services.Interfaces;

public interface IEmailService
{
    Task SendTaskAssignmentEmailAsync(string recipientEmail, string taskTitle, string taskId);
    Task SendTaskDueReminderEmailAsync(string recipientEmail, string taskTitle, DateTime dueDate);
    Task SendTaskOverdueEmailAsync(string recipientEmail, string taskTitle, DateTime dueDate);
    Task SendTaskStatusChangeEmailAsync(string recipientEmail, string taskTitle, string oldStatus, string newStatus);
}