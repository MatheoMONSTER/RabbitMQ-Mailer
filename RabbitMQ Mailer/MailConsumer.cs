using MimeKit;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MailKit.Net.Smtp;
using System.Net.Mail;
using System.Text;

namespace RabbitMQ_Mailer
{
    public class MailConsumer
    {
        private readonly IModel _channel;

        public MailConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "mail_queue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var mail = JsonConvert.DeserializeObject<Mail>(message);
                SendMail(mail);
            };
            _channel.BasicConsume(queue: "mail_queue",
                autoAck: true,
                consumer: consumer);
        }

        private void SendMail(Mail mail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("from@example.com", "Name"));
            message.To.Add(new MailboxAddress("to@example.com", "Name"));
            message.Subject = mail.Subject;
            message.Body = new TextPart("plain")
            {
                Text = mail.Body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("your-email@gmail.com", "your-password");

                client.Send(message);

                client.Disconnect(true);
            }
        }

    }
}
