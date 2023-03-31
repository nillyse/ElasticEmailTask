using ElasticEmailTask.Models;

namespace ElasticEmailTask.Interfaces
{
    public interface IElasticEmailService
    {
        List<SendEmailRequest> ReadFromCsvFile(string path);
        List<string> SendEmail(List<SendEmailRequest> emails);
    }
}