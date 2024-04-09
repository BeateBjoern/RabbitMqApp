using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Services;
using Interfaces;
using Models;

namespace Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly IProducerService _producerService;

        private readonly ILogger<ProducerController> _logger;

        // Constructor for controller taking a ProducerService instance as input 
        public ProducerController(IProducerService service, ILogger<ProducerController> logger)
        {
            _producerService = service; //Use local service instance 
            _logger = logger; //Use local logger instance

            // Get the host name of the current machine
            var hostName = System.Net.Dns.GetHostName();

            // Get the IP addresses associated with the host name
            var ips = System.Net.Dns.GetHostAddresses(hostName);

            // Get the first IPv4 address from the list of IP addresses
            var _ipaddr = ips.First().MapToIPv4().ToString();

            // Log the information about the service's IP address
            _logger.LogInformation(1, $"Producer Service responding from {_ipaddr}");
        }



    [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            try
            {        
                var messages = await _producerService.GetMessagesAsync();
                _logger.LogInformation("Messages retrieved successfully");
                return Ok(messages);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
      
        }


        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            if (message.Counter <= 0 || string.IsNullOrEmpty(message.Value))
            {   
                _logger.LogError("  Invalid request body\n\n");
                return BadRequest("Invalid request body");
            }
          

            // Call the service method with the extracted values
            await _producerService.CreateMessageAsync(message);
            _logger.LogInformation("  Message sent successfully\n\n");
            return Ok($"Message sent successfully with value: {message.Value} and counter: {message.Counter}");
        }



        [HttpPost("{number}")]
        public async Task<IActionResult> TriggerProducer(int number)
        {
            // Triggering the producer logic to send a message with post request 
            try{
                await _producerService.CreateMessagesAsync(number);
                _logger.LogInformation("  Messages sent successfully\n\n");
                return Ok("Message sent successfully");
                
            }
            catch{
                
                _logger.LogError("  Error sending messages\n\n");
                return BadRequest("Error sending messages");
            }
        }


    }

    

