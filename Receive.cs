using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Receive
{
    class Receive
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
                List<RequestQ> lstrequ = new List<RequestQ>();
                channel.QueueDeclare(queue: "hello1",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                lstrequ = JsonSerializer.Deserialize<List<RequestQ>>(message);
                Console.WriteLine(" [x] Received {0}", message);
                channel.BasicAck(deliveryTag: 1, multiple: true);
            };
            channel.BasicConsume(queue: "hello1",
                                 autoAck: false,
                                 consumerTag : "receiver",
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

        public class RequestQ
        {
            public int REQUEST_NUM;

            public bool Received_by_RefreshWS;

            public bool Received_by_Mobility_BackEnd;
        }
    }
}
