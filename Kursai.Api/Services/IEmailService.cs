namespace Kursai.Api.Services
{
    public interface IEmailService
    {
        Task SendPurchaseConfirmationAsync(string toEmail, string userName, string courseTitle, decimal price, string? attachmentUrl);
        Task SendNewCourseNotificationAsync(string toEmail, string sellerName, string courseTitle);
        Task SendRatingReceivedAsync(string toEmail, string courseTitle, int rating, string? comment);
    }
}
