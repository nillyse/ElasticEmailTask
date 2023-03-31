using ElasticEmailTask.Models;
using ElasticEmailTask.Enums;
using CsvHelper.Configuration;

public class SendEmailRequestMap : ClassMap<SendEmailRequest>
{
    public SendEmailRequestMap()
    {
        Map(ser => ser.From);
        Map(ser => ser.To);
        Map(ser => ser.Subject);
        Map(ser => ser.Body);
        Map(ser => ser.Format).Convert((f) => {
            var value = f.Row[nameof(SendEmailRequest.Format)]!.ToLower();
            if (value == "text")
                return EmailFormat.Text;
            else if (value == "html")
                return EmailFormat.Html;
            throw new ArgumentException($"Invalid format parameter. Expected 'text' or 'html' but got {value} instead.");

        });
    }
}
