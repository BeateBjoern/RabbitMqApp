using System.Threading.Tasks;
namespace Interfaces;
using RabbitMQ.Client;
using Models;
using Microsoft.AspNetCore.Mvc;


public interface IProducerService
{
    public Task CreateMessagesAsync(int numberOfMessages); 
    public Task CreateMessageAsync(Message message);
    public Task<List<Message>> GetMessagesAsync();
}