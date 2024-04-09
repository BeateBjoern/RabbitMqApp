using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MongoDB.Driver;   
using Models;
using System.Text.Json;
using System.Text;
using Data; 
using Services; 
using Interfaces;
 

namespace Consumer1;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    //..this is where the RabbitMQConnectionFactory is injected for the Worker class (from the IRabbitMQConnectionFactory interface). 
    //..this enables the Worker class to create a connection to RabbitMQ from RabbitMQConnection facotyr class VIA the interface 
    private readonly IRabbitMQConnectionFactory _factory;
    private readonly IMongoDBContext _mongoDbContext;
    private readonly MessageHandler _messageHandler; 
    

    //everytime the Worker class is called upon (from program.cs on app start) it is constructed with the following parameters
    //This provides the Worker class with the necessary dependencies to interact with RabbitMQ, MongoDB and the MessageHandler class
    public Worker(ILogger<Worker> logger, MongoDBContext mongoDbContext, IRabbitMQConnectionFactory factory, MessageHandler messageHandler)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
        _factory = factory; 
        _messageHandler = messageHandler;

            // Get the host name of the current machine
            var hostName = System.Net.Dns.GetHostName();

            // Get the IP addresses associated with the host name
            var ips = System.Net.Dns.GetHostAddresses(hostName);

            // Get the first IPv4 address from the list of IP addresses
            var _ipaddr = ips.First().MapToIPv4().ToString();

            // Log the information about the service's IP address
            _logger.LogInformation(1, $"Producer Service responding from {_ipaddr}");

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        await DeclareQueue(channel); // Declaring the queue
        await ConsumeMessage(channel, stoppingToken); // Consuming the message

    }

    public async Task DeclareQueue(IModel channel)
    {
        channel.QueueDeclare(queue: "messages",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public async Task ConsumeMessage(IModel channel, CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var messageObj = JsonSerializer.Deserialize<Message>(message);

            Console.WriteLine($"Received {message}");

            await _messageHandler.HandleMessages(messageObj, ea.DeliveryTag, channel);
        };

        channel.BasicConsume(queue: "messages", autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

   
   
}
