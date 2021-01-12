using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_queue2",
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    channel.BasicQos(0, 1, false);
                    
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);

                        Console.WriteLine(" [x] Done");

                        // Properly acknowledging once everything is done
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    // autoAck -> Auto acknowledgment (may be bad if the consumer dies)
                    channel.BasicConsume(queue: "task_queue2", autoAck: false, consumer: consumer);

                    Console.WriteLine(" Press enter to exit ");
                    Console.ReadLine();
                }
            }
        }
    }
}
