using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TaskMaster.Configuration;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendTaskAssignmentEmailAsync(
        string recipientEmail,
        string taskTitle,
        string taskId)
    {
        var subject = $"New Task Assigned: {taskTitle}";
        var body = $@"
            <html>
            <body>
                <h2>You have been assigned a new task</h2>
                <p><strong>Task:</strong> {taskTitle}</p>
                <p><strong>Task ID:</strong> {taskId}</p>
                <p>Please log in to TaskMaster to view the task details and manage your work.</p>
                <br/>
                <p>Best regards,<br/>TaskMaster Team</p>
            </body>
            </html>
        ";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendTaskDueReminderEmailAsync(
        string recipientEmail,
        string taskTitle,
        DateTime dueDate)
    {
        var subject = $"Reminder: Task Due Soon - {taskTitle}";
        var body = $@"
            <html>
            <body>
                <h2>Task Due Date Reminder</h2>
                <p><strong>Task:</strong> {taskTitle}</p>
                <p><strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}</p>
                <p>This task is due soon. Please ensure it is completed on time.</p>
                <br/>
                <p>Best regards,<br/>TaskMaster Team</p>
            </body>
            </html>
        ";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendTaskOverdueEmailAsync(
        string recipientEmail,
        string taskTitle,
        DateTime dueDate)
    {
        var subject = $"Overdue Task: {taskTitle}";
        var body = $@"
            <html>
            <body>
                <h2>Task Overdue</h2>
                <p><strong>Task:</strong> {taskTitle}</p>
                <p><strong>Was Due:</strong> {dueDate:MMMM dd, yyyy}</p>
                <p style=""color: red;"">This task is now overdue. Please complete it as soon as possible.</p>
                <br/>
                <p>Best regards,<br/>TaskMaster Team</p>
            </body>
            </html>
        ";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendTaskStatusChangeEmailAsync(
        string recipientEmail,
        string taskTitle,
        string oldStatus,
        string newStatus)
    {
        var subject = $"Task Status Updated: {taskTitle}";
        var body = $@"
            <html>
            <body>
                <h2>Task Status Changed</h2>
                <p><strong>Task:</strong> {taskTitle}</p>
                <p><strong>Previous Status:</strong> {oldStatus}</p>
                <p><strong>New Status:</strong> {newStatus}</p>
                <br/>
                <p>Best regards,<br/>TaskMaster Team</p>
            </body>
            </html>
        ";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    private async Task SendEmailAsync(string recipientEmail, string subject, string body)
    {
        try
        {
            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.SmtpPort,
                Credentials = new NetworkCredential(
                    _emailSettings.Username,
                    _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(
                    _emailSettings.SenderEmail,
                    _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(recipientEmail);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation(
                "Email sent successfully to {Recipient} with subject: {Subject}",
                recipientEmail,
                subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {Recipient} with subject: {Subject}",
                recipientEmail,
                subject);
            
        }
    }
}