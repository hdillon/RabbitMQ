using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace EmitLog
{
    class EmitLog
    {
        static void Main(string[] args)
        {
            string hostName = System.Configuration.ConfigurationManager.AppSettings["RabbitMQHostName"];
            string port = System.Configuration.ConfigurationManager.AppSettings["RabbitMQPort"];
            var factory = new ConnectionFactory() { HostName = hostName, Port = int.Parse(port) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "topic");

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "logs",
                                     routingKey: "routingKey2",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "info: Some log message..");
        }
    }
}
