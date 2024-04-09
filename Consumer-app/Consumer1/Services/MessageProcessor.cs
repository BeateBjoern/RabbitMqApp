 using Models; 
  
  namespace Services; 
  public class MessageProcessor {

  
  public int ProcessMessageForValidTimestamp(Message messageObj)
        {
            if (messageObj == null || messageObj.Value == null)
                return -1; // Invalid message

            if (DateTime.UtcNow - messageObj.Timestamp >= TimeSpan.FromMinutes(1))
                return 0; // Message is too old

            if (messageObj.Timestamp.Second % 2 == 0)
                return 1; // Save message to database and CSV
            else
                return 2; // Requeue modified message
        }

        
  }