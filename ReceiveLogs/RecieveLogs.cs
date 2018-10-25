using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLogs
{
    class RecieveLogs
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
                //Random queue name
                var queueName = channel.QueueDeclare().QueueName;
                //Binding queue with exchange
                channel.QueueBind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "routingKey1");
                
                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] {0}", message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);


                Console.WriteLine("Add new binding routingKey2? (S/N)");
                string response = Console.ReadLine();

                if(response.ToUpper() == "S")
                    channel.QueueBind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "routingKey2");

                Console.WriteLine("Delete binding routingKey1? (S/N)");
                response = Console.ReadLine();

                if(response.ToUpper() == "S")
                    channel.QueueUnbind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "routingKey1");

                Console.ReadKey();
                channel.QueueUnbind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "routingKey2");

                Console.ReadKey();
                channel.QueueBind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "routingKey3");

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
