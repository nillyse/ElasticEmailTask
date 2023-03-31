using ElasticEmailTask.Enums;
using ElasticEmailTask.Interfaces;
using ElasticEmailTask.Models;

namespace ElasticEmailTask
{
    public class Main
    {
        private List<string> _options = new List<string>()
            {
                "Send email",
                "Send email from csv file",
                "Quit"
            };
        private List<string> _innerOptions = new List<string>()
            {
                "Send",
                "Add another",
                "View",
                "Cancel"
            };
        private List<string> _deleteOptions = new List<string>()
            {
                "Yes",
                "No"
            };
        private List<string> _formatOptions = new List<string>()
            {
                "Html",
                "Text"
            };

        private List<SendEmailRequest> _sendEmailRequests = new List<SendEmailRequest>();
        private IElasticEmailService _elasticEmailService;

        public Main(IElasticEmailService elasticEmailService) 
        {
            _elasticEmailService = elasticEmailService;
        }

        public int Run()
        {
            int option = 1, innerOption = 2, format = 1;
            while (option != 3)
            {
                _sendEmailRequests.Clear();
                option = DisplayAndSelectOption(_options, header: "MENU:");
                if (option == 1)
                {
                    while (true)
                    {
                        Console.Clear();
                        SendEmailRequest sendEmailRequest = new SendEmailRequest();
                        Console.WriteLine("1. Input From: ");
                        sendEmailRequest.From = Console.ReadLine();
                        Console.WriteLine("2. Input To: ");
                        sendEmailRequest.To = Console.ReadLine();
                        Console.WriteLine("3. Input Subject: ");
                        sendEmailRequest.Subject = Console.ReadLine();
                        Console.WriteLine("4. Choose format: ");
                        format = DisplayAndSelectOption(_formatOptions, false);
                        sendEmailRequest.Format = format == 1 ? EmailFormat.Html : EmailFormat.Text;
                        Console.WriteLine("5. Input body: ");
                        sendEmailRequest.Body = Console.ReadLine();
                        Console.Clear();
                        _sendEmailRequests.Add(sendEmailRequest);
                        if (selectInnerOption())
                            break;

                    }
                }
                else if (option == 2)
                {
                    Console.Clear();
                    Console.WriteLine("Input path to csv file: ");
                    var path = Console.ReadLine();
                    _sendEmailRequests = _elasticEmailService.ReadFromCsvFile(path);
                    var results = _elasticEmailService.SendEmail(_sendEmailRequests);
                    foreach (var result in results)
                    {
                        Console.WriteLine(result);
                    }
                    Console.WriteLine("Enter any key to continue...");
                    Console.ReadKey();
                }
            }
            return 0;
        }
        int DisplayAndSelectOption(List<string> options, bool clearPrevious = true, string header = "")
        {
            if (clearPrevious)
                Console.Clear();
            var option = 1;
            (int left, int top) = Console.GetCursorPosition();
            var decorator = "\u001b[32m>\u001b[32m ";
            ConsoleKeyInfo key;
            bool isSelected = false;
            while (!isSelected)
            {
                Console.SetCursorPosition(left, top);
                var length = options.Count();
                if (header != "")
                    Console.WriteLine(header);
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine($"{(option == i + 1 ? decorator : "") + options[i]}\u001b[0m   ");
                }
                key = Console.ReadKey(false);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        option = option == 1 ? length : option - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        option = option == length ? 1 : option + 1;
                        break;
                    case ConsoleKey.Enter:
                        isSelected = true;
                        return option;
                }
            }
            return 0;
        }
        bool selectInnerOption()
        {
            int innerOption = DisplayAndSelectOption(_innerOptions, header: "MENU:");
            if (innerOption == 1)
            {
                var results = _elasticEmailService.SendEmail(_sendEmailRequests);
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
                Console.WriteLine("Enter any key to continue...");
                Console.ReadKey();
                _sendEmailRequests.Clear();
                return true;
            }
            else if (innerOption == 3)
            {
                Console.Clear();
                foreach (var emailRequest in _sendEmailRequests)
                {
                    Console.WriteLine(emailRequest.ToString());
                }
                Console.WriteLine("Enter any key to continue...");
                Console.ReadKey();
                selectInnerOption();
            }
            else if (innerOption == 4)
            {
                Console.WriteLine("Are you sure? (this operation delete all written emails)");
                if (DisplayAndSelectOption(_deleteOptions, false) == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}