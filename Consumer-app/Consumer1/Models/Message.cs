
namespace Models; 
public class Message
{
    public string Value { get; set; } // message content 
    public int Counter { get; set; } //Counter for the message (for reqeueing messages)
    public DateTime Timestamp { get; set; } // Timestamp for the message

}