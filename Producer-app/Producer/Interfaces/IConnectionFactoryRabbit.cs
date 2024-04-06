using RabbitMQ.Client;

namespace Interfaces;

public interface IConnectionFactoryRabbit
{
    IConnection CreateConnection(); 
}