using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Producer
{
    class Send
    {
        static void Main(string[] args)
        {
            string hostName = System.Configuration.ConfigurationManager.AppSettings["RabbitMQHostName"];
            string port = System.Configuration.ConfigurationManager.AppSettings["RabbitMQPort"];
            var factory = new ConnectionFactory() { HostName = hostName, Port = int.Parse(port)};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "MyQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "Custom message";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "MyQueue",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
