using Moq;
using ElasticEmailTask.Models;
using ElasticEmailTask.Services;
using ElasticEmail.Api;
using ElasticEmail.Client;
using ElasticEmail.Model;
using ElasticEmailTask.Enums;

namespace ElasticEmailTask.Tests.Services
{
    public class ElasticEmailServiceTests
    {
        private readonly Mock<IEmailsApi> _mockApiInstance;
        private readonly ElasticEmailService _elasticEmailService;

        public ElasticEmailServiceTests()
        {

            _mockApiInstance = new Mock<IEmailsApi>();
            _elasticEmailService = new ElasticEmailService(_mockApiInstance.Object);
        }

        [Fact]
        public void SendEmail_WhenCorrectEmailData_ReturnSuccessMessage()
        {
            // Arrange
            var email = new SendEmailRequest
            {
                To = "test@example.com",
                From = "sender@example.com",
                Subject = "Test Subject",
                Body = "Test Body",
                Format = EmailFormat.Html
            };

            var emailSendResult = new EmailSend
            {
                TransactionID = Guid.NewGuid().ToString()
            };

            _mockApiInstance.Setup(x => x.EmailsPost(It.IsAny<EmailMessageData>(), default)).Returns(emailSendResult);

            // Act
            var result = _elasticEmailService.SendEmail(new List<SendEmailRequest> { email });

            // Assert
            Assert.Single(result);
            Assert.Contains($"Email from {email.From} to {email.To} with transaction id '{emailSendResult.TransactionID}' was sent successfully", result[0]);
            _mockApiInstance.Verify(x => x.EmailsPost(It.IsAny<EmailMessageData>(), default), Times.Once);
        }

        [Fact]
        public void SendEmail_WhenApiThrowsAnError_ReturnsFailureMessage()
        {
            // Arrange
            var email = new SendEmailRequest
            {
                To = "test@example.com",
                From = "sender@example.com",
                Subject = "Test Subject",
                Body = "Test Body",
                Format = EmailFormat.Html
            };

            _mockApiInstance.Setup(x => x.EmailsPost(It.IsAny<EmailMessageData>(), default)).Throws(new ApiException(400, "Bad Request"));

            // Act
            var result = _elasticEmailService.SendEmail(new List<SendEmailRequest> { email });

            // Assert
            Assert.Single(result);
            Assert.Contains($"Email to {email.To} from {email.From}", result[0]);
            Assert.Contains("failed to send", result[0]);
            _mockApiInstance.Verify(x => x.EmailsPost(It.IsAny<EmailMessageData>(), default), Times.Once);
        }

        [Fact]
        public void ReadFromCsvFile_WhenCalledWithCsvFilePath_ShouldReturnListOfSendEmailRequest()
        {
            // Arrange
            var csvFilePath = "test.csv";
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(new List<SendEmailRequest>
                {
                    new SendEmailRequest
                    {
                        From = "from@example.com",
                        To = "to1@example.com",
                        Subject = "Test Email 1",
                        Body = "This is a test email 1.",
                        Format = Enums.EmailFormat.Text
                    },
                    new SendEmailRequest
                    {
                        From = "from@example.com",
                        To = "to2@example.com",
                        Subject = "Test Email 2",
                        Body = "This is a test email 2.",
                        Format = Enums.EmailFormat.Html
                    }
                });
            }

            // Act
            var result = _elasticEmailService.ReadFromCsvFile(csvFilePath);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("to1@example.com", result[0].To);
            Assert.Equal("Test Email 1", result[0].Subject);
            Assert.Equal("This is a test email 1.", result[0].Body);
            Assert.Equal(Enums.EmailFormat.Text, result[0].Format);
            Assert.Equal("to2@example.com", result[1].To);
            Assert.Equal("Test Email 2", result[1].Subject);
            Assert.Equal("This is a test email 2.", result[1].Body);
            Assert.Equal(Enums.EmailFormat.Html, result[1].Format);

            // Clean up
            File.Delete(csvFilePath);
        }

        [Fact]
        public void ReadFromCsvFile_WhenFileHasIncorrectFormat_ShouldThrowAnException()
        {
            // Arrange
            var csvFilePath = "test.csv";
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(new List<object>
                {
                    new
                    {
                        From = "from@example.com",
                        To = "to1@example.com",
                        Subject = "Test Email 1",
                        Body = "This is a test email 1.",
                        Format = "incorrect format"
                    },
                });
            }

            // Act
            Action resultAction = () => _elasticEmailService.ReadFromCsvFile(csvFilePath);

            // Assert
            Assert.Throws<CsvHelper.ReaderException>(resultAction);

            // Clean up
            File.Delete(csvFilePath);
        }
    }
}