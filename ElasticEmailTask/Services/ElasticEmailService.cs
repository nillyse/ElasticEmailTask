using ElasticEmailTask.Models;
using ElasticEmailTask.Enums;
using ElasticEmailTask.Interfaces;
using System.Globalization;
using CsvHelper;
using ElasticEmail.Api;
using ElasticEmail.Client;
using ElasticEmail.Model;

namespace ElasticEmailTask.Services
{
    public class ElasticEmailService : IElasticEmailService
    {
        private readonly IEmailsApi _apiInstance;

        public ElasticEmailService(IEmailsApi apiInstance)
        {
            _apiInstance = apiInstance;
        }

        public List<string> SendEmail(List<SendEmailRequest> emails)
        {
            List<string> results = new List<string>();
            foreach (var email in emails)
            {
                var emailMessageData = new EmailMessageData(new List<EmailRecipient>()
                {
                    new EmailRecipient(email.To)
                }
);
                emailMessageData.Content = new EmailContent(
                    body: new List<BodyPart>() { new BodyPart(email.Format == EmailFormat.Html ? BodyContentType.HTML : BodyContentType.PlainText, email.Body) },
                    from: email.From,
                    subject: email.Subject
                );

                try
                {
                    EmailSend result = _apiInstance.EmailsPost(emailMessageData);
                    results.Add($"Email from {email.From} to {email.To} with transaction id '{result.TransactionID}' was sent successfully");

                }
                catch (ApiException e)
                {
                    results.Add($"Email to {email.To} from {email.From} failed to send");
                }
            }
            return results;
        }

        public List<SendEmailRequest> ReadFromCsvFile(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<SendEmailRequestMap>();
                return csv.GetRecords<SendEmailRequest>().ToList();
            }
        }
    }
}
