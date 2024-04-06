using Models;

namespace Interfaces;

public interface IMongoDBContext
{
    Task InsertMessageAsync(Message message);

}