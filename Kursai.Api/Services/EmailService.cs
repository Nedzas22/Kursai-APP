using SendGrid;
using SendGrid.Helpers.Mail;

namespace Kursai.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _apiKey = configuration["SendGridSettings:ApiKey"] ?? throw new ArgumentNullException("SendGrid API Key is not configured");
            _fromEmail = configuration["SendGridSettings:FromEmail"] ?? "noreply@kursai.lt";
            _fromName = configuration["SendGridSettings:FromName"] ?? "Kursai Platform";
            _logger = logger;
        }

        public async Task SendPurchaseConfirmationAsync(string toEmail, string userName, string courseTitle, decimal price, string? attachmentUrl)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, userName);
            var subject = $"Purchase Confirmation - {courseTitle}";

            var plainTextContent = $@"Hello, {userName}!

Thank you for your purchase!

Course Information:
- Title: {courseTitle}
- Price: €{price:F2}
- Purchase Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm}

{(string.IsNullOrEmpty(attachmentUrl) ? "" : $"You can download the course from your Library.")}

Best regards,
Kursai Team";

            var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; margin-top: 20px; }}
        .course-info {{ background-color: white; padding: 15px; margin: 15px 0; border-left: 4px solid #4CAF50; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>?? Purchase Successful!</h1>
        </div>
        <div class='content'>
            <p>Hello, <strong>{userName}</strong>!</p>
            <p>Thank you for your purchase on Kursai Platform!</p>
            
            <div class='course-info'>
                <h3>?? Course Information:</h3>
                <p><strong>Title:</strong> {courseTitle}</p>
                <p><strong>Price:</strong> €{price:F2}</p>
                <p><strong>Purchase Date:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm}</p>
            </div>

            {(string.IsNullOrEmpty(attachmentUrl) ? "" : "<p>? You can access your course in the <strong>Library</strong> section of the app.</p>")}
            
            <p>If you have any questions, feel free to contact us.</p>
        </div>
        <div class='footer'>
            <p>© 2024 Kursai Platform | All rights reserved</p>
        </div>
    </div>
</body>
</html>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            try
            {
                var response = await client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Purchase confirmation email sent to {toEmail}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send email to {toEmail}. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending purchase confirmation email to {toEmail}");
            }
        }

        public async Task SendNewCourseNotificationAsync(string toEmail, string sellerName, string courseTitle)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, sellerName);
            var subject = "Your Course Has Been Created Successfully!";

            var plainTextContent = $@"Hello, {sellerName}!

Your course ""{courseTitle}"" has been successfully created and is now visible on Kursai Platform.

Other users can now browse and purchase your course.

Best regards,
Kursai Team";

            var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; margin-top: 20px; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>? Course Created!</h1>
        </div>
        <div class='content'>
            <p>Hello, <strong>{sellerName}</strong>!</p>
            <p>Your course <strong>""{courseTitle}""</strong> has been successfully created and is now live on the platform.</p>
            <p>?? Other users can now browse and purchase your course!</p>
            <p>Good luck!</p>
        </div>
        <div class='footer'>
            <p>© 2024 Kursai Platform | All rights reserved</p>
        </div>
    </div>
</body>
</html>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            try
            {
                var response = await client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"New course notification email sent to {toEmail}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send email to {toEmail}. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending new course notification email to {toEmail}");
            }
        }

        public async Task SendRatingReceivedAsync(string toEmail, string courseTitle, int rating, string? comment)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var subject = $"New Rating for Your Course: {courseTitle}";

            var stars = new string('?', rating);

            var plainTextContent = $@"Hello!

Your course ""{courseTitle}"" has received a new rating!

Rating: {stars} ({rating}/5)
{(string.IsNullOrEmpty(comment) ? "" : $"Comment: {comment}")}

Best regards,
Kursai Team";

            var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #FF9800; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; margin-top: 20px; }}
        .rating {{ font-size: 24px; text-align: center; margin: 20px 0; }}
        .comment {{ background-color: white; padding: 15px; margin: 15px 0; border-left: 4px solid #FF9800; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>? New Rating!</h1>
        </div>
        <div class='content'>
            <p>Hello!</p>
            <p>Your course <strong>""{courseTitle}""</strong> has received a new rating!</p>
            
            <div class='rating'>
                {stars} ({rating}/5)
            </div>

            {(string.IsNullOrEmpty(comment) ? "" : $@"
            <div class='comment'>
                <p><strong>?? Comment:</strong></p>
                <p>{comment}</p>
            </div>")}
            
            <p>Keep up the great work! ??</p>
        </div>
        <div class='footer'>
            <p>© 2024 Kursai Platform | All rights reserved</p>
        </div>
    </div>
</body>
</html>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            try
            {
                var response = await client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Rating notification email sent to {toEmail}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send email to {toEmail}. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending rating notification email to {toEmail}");
            }
        }
    }
}
