using RabbitMQ.Client;

namespace Interfaces;

public interface IRabbitMQConnectionFactory
{
    IConnection CreateConnection(); 

}