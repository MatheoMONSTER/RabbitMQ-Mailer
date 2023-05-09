namespace RabbitMQ_Mailer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var producer = new MailProducer();
            producer.SendMessage(new Mail
            {
                From = "your-email@gmail.com",
                To = "recipient-email@example.com",
                Subject = "Test message",
                Body = "This is a test message sent via RabbitMQ"
            });

            var consumer = new MailConsumer();
            consumer.StartConsuming();
        }
    }
}