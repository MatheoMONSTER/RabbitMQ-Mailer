using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMQ_Mailer
{
    public class MailProducer
    {
        private readonly IModel _channel;

        public MailProducer()
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
        
        public void SendMessage(Mail mail)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mail));
            _channel.BasicPublish(exchange: "",
                                  routingKey: "mail_queue",
                                  basicProperties: null,
                                  body: body);
        }
    }
}
