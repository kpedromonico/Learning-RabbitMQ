using RabbitMQ.Client;
using System;
using System.Text;

namespace EmitLog
{
    class EmitLog
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("logger", ExchangeType.Fanout);

                    var queueName = channel.QueueDeclare().QueueName;

                    var msg = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(msg);

                    channel.BasicPublish(exchange: "logger", routingKey: "", basicProperties: null, body: body);
                    Console.WriteLine(" [x] Sent {0}", msg);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }



        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
