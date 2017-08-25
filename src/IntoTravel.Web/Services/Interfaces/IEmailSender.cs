using System.Threading.Tasks;

namespace IntoTravel.Web.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
