using TaskMaster.Repositories.Interfaces;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Services.BackgroundServices;

public class DeadlineNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeadlineNotificationService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public DeadlineNotificationService(
        IServiceProvider serviceProvider,
        ILogger<DeadlineNotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Deadline Notification Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndNotifyDeadlines();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking task deadlines");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Deadline Notification Service stopped");
    }

    private async Task CheckAndNotifyDeadlines()
    {
        using var scope = _serviceProvider.CreateScope();
        var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var now = DateTime.UtcNow;
        var tomorrow = now.AddDays(1);

        var tasksDueSoon = await taskRepository.GetTasksDueSoonAsync(now, tomorrow);
        var overdueTasks = await taskRepository.GetOverdueTasksAsync(now);

        // Collect all unique assigned user IDs across both lists — single user query
        var allTasks = tasksDueSoon.Concat(overdueTasks).ToList();
        var userIds = allTasks
            .Where(task => task.AssignedToUserId.HasValue)
            .Select(task => task.AssignedToUserId.GetValueOrDefault())
            .Distinct()
            .ToList();

        if (userIds.Count == 0) return;

        var users = (await userRepository.GetByIdsAsync(userIds))
            .ToDictionary(u => u.Id);

        // Send due-soon reminders
        foreach (var task in tasksDueSoon)
        {
            if (task.AssignedToUserId is not Guid assignedUserId ||
                task.DueDate is not DateTime dueDate ||
                !users.TryGetValue(assignedUserId, out var user))
                continue;

            await emailService.SendTaskDueReminderEmailAsync(
                user.Email,
                task.Title,
                dueDate);

            _logger.LogInformation(
                "Sent due date reminder for task {TaskId} to user {UserEmail}",
                task.Id,
                user.Email);
        }

        // Send overdue notifications
        foreach (var task in overdueTasks)
        {
            if (task.AssignedToUserId is not Guid assignedUserId ||
                task.DueDate is not DateTime dueDate ||
                !users.TryGetValue(assignedUserId, out var user))
                continue;

            await emailService.SendTaskOverdueEmailAsync(
                user.Email,
                task.Title,
                dueDate);

            _logger.LogInformation(
                "Sent overdue notification for task {TaskId} to user {UserEmail}",
                task.Id,
                user.Email);
        }

        if (tasksDueSoon.Any() || overdueTasks.Any())
        {
            _logger.LogInformation(
                "Processed {DueSoonCount} due soon and {OverdueCount} overdue tasks",
                tasksDueSoon.Count,
                overdueTasks.Count);
        }
    }
}
