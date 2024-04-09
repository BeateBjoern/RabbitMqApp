using RabbitMQ.Client;
using System.Text.Json;
using System.Threading;
using Interfaces;
using Models;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;

namespace Services
{
    public class ProducerService : IProducerService
    {
        private readonly IConnectionFactory _factory;
        private readonly string _csvFilePath;

        public ProducerService(IConnectionFactory factory, string csvFilePath)
        {
            _factory = factory;
            _csvFilePath = csvFilePath;
        }

        public async Task<List<Message>> GetMessagesAsync()
        {
            try
            {
                // Read the CSV file
                using (var reader = new StreamReader(_csvFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Parse the CSV data into a list of objects
                    var records = csv.GetRecords<Message>().ToList();

                    // Return the parsed data
                    return records;
                }
            }
            catch (Exception ex)
            {
                // If an error occurs, throw an exception with a descriptive message
                throw new Exception($"An error occurred while reading CSV file: {ex.Message}");
            }
        }

        public async Task CreateMessageAsync(Message message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the queue
            DeclareQueue(channel);

            // Create a new message object with a unique id and timestamp
            var newMessage = new Message
            {
                Counter = 1,
                Value = $"[{message.Value}] ",
                Timestamp = DateTime.UtcNow
            };

            // Publish the message with a short delay
            PublishMessage(newMessage, channel);
            Thread.Sleep(1500);

            // Close the connection and channel
            return;
        }

        public async Task CreateMessagesAsync(int numOfMessages)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the queue
            DeclareQueue(channel);

            for (var i = 1; i <= numOfMessages; i++)
            {
                // Create a new message object with a unique id and timestamp
                var message = new Message
                {
                    Counter = 1,
                    Value = $"[{i}] ",
                    Timestamp = DateTime.UtcNow
                };

                // Publish the message with a short delay
                PublishMessage(message, channel);
                Thread.Sleep(1500);
            } 

            // Close the connection and channel
            return;
        }

        public void DeclareQueue(IModel channel)
        {
            channel.QueueDeclare(queue: "messages",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }

        public void PublishMessage(Message message, IModel channel)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                routingKey: "messages",
                                basicProperties: null,
                                body: body);

            Console.WriteLine($"Published message with value: {message.Value}, Counter: {message.Counter}, Timestamp: [{message.Timestamp}]");
        }
    }
}
