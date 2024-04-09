 using RabbitMQ.Client;
 using Models;
 namespace Interfaces; 
 public interface IMessageHandler
    {

        Task HandleMessages(Message messageObj, ulong deliveryTag, IModel channel);
    }