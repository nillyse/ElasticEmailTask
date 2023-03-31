using ElasticEmailTask.Enums;

namespace ElasticEmailTask.Models;

public class SendEmailRequest
{
    public string From { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public EmailFormat Format { get; set; }
    public string Body { get; set; }

    public override string ToString()
    {
        return $"{From} -> {To} \n" + $"Subject: {Subject}\n" + $"Format: {Format}\n" + $"{Body}\n";
            
    }

}
