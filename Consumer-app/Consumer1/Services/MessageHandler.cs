using Data; 
using Models; 
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Interfaces;
using CsvHelper;
using CsvHelper.Configuration;  


namespace Services
{ 
    public class MessageHandler : IMessageHandler
    {

        // List of dependencies
        private readonly ILogger<MessageHandler> _logger;
        private readonly MongoDBContext _mongoDbContext;
        private readonly CsvWriterService _csvWriterService;
        private readonly MessageProcessor _messageProcessor;

        // Initializes a new instance of the MessageHandler class w/ dependencies
        public MessageHandler(ILogger<MessageHandler> logger, MongoDBContext mongoDbContext, CsvWriterService csvWriterService, MessageProcessor messageProcessor)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
            _csvWriterService = csvWriterService;
            _messageProcessor = messageProcessor;
        }
      
        // Method to handle incoming messages
        public async Task HandleMessages(Message messageObj, ulong deliveryTag, IModel channel)
        {
            try
            {
                // Process the message to determine the action
                int action = _messageProcessor.ProcessMessageForValidTimestamp(messageObj);
                switch (action)
                {
                    case 0:
                        // Message is too old and deleted from queue
                        Console.WriteLine("Message is too old and deleted from queue \n -----------------------------------------------------------");
                        channel.BasicAck(deliveryTag, false);
                        
                        break;
                    case 1:
                        // Save message to database
                        await _mongoDbContext.InsertMessageAsync(messageObj);
                        Console.WriteLine("Saved message to database \n -----------------------------------------------------------");
                        channel.BasicAck(deliveryTag, false);
                        _csvWriterService.WriteToCsv(new List<Message> { messageObj });
                        Console.WriteLine("Message written to CSV \n -----------------------------------------------------------");

                        break;
                    case 2:
                        // If second of timestamp is odd, requeue 
                        
                        Console.WriteLine(" Message requeued: " + messageObj.Counter + " time(s)");
                        messageObj.Counter += 1;
                        messageObj.Timestamp = DateTime.UtcNow;

                        var modifiedMessage = JsonSerializer.Serialize(messageObj);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: string.Empty,
                                            routingKey: "messages",
                                            basicProperties: null,
                                            body: Encoding.UTF8.GetBytes(modifiedMessage));

                        Thread.Sleep(1500);

                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    default:
                        // Invalid message
                        Console.WriteLine("Invalid message");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during message processing
                _logger.LogInformation("An error occurred while processing the message: " + ex.Message);
            }
        }
    }
}
