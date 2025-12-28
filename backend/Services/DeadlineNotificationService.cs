using Microsoft.EntityFrameworkCore;
using TaskMaster.Data;
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
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var now = DateTime.UtcNow;
        var tomorrow = now.AddDays(1);

        // Get tasks due within 24 hours
        var tasksDueSoon = await context.Tasks
            .Where(t => t.DueDate.HasValue &&
                       t.DueDate.Value > now &&
                       t.DueDate.Value <= tomorrow &&
                       t.Status != "COMPLETED" &&
                       t.AssignedToUserId != null)
            .ToListAsync();

        foreach (var task in tasksDueSoon)
        {
            var user = await userService.GetUserByIdAsync(task.AssignedToUserId!.Value);
            if (user != null)
            {
                await emailService.SendTaskDueReminderEmailAsync(
                    user.Email,
                    task.Title,
                    task.DueDate!.Value);

                _logger.LogInformation(
                    "Sent due date reminder for task {TaskId} to user {UserEmail}",
                    task.Id,
                    user.Email);
            }
        }

        // Get overdue tasks
        var overdueTasks = await context.Tasks
            .Where(t => t.DueDate.HasValue &&
                       t.DueDate.Value < now &&
                       t.Status != "COMPLETED" &&
                       t.AssignedToUserId != null)
            .ToListAsync();

        foreach (var task in overdueTasks)
        {
            var user = await userService.GetUserByIdAsync(task.AssignedToUserId!.Value);
            if (user != null)
            {
                await emailService.SendTaskOverdueEmailAsync(
                    user.Email,
                    task.Title,
                    task.DueDate!.Value);

                _logger.LogInformation(
                    "Sent overdue notification for task {TaskId} to user {UserEmail}",
                    task.Id,
                    user.Email);
            }
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