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

        // Constructor for controller taking a ProducerService instance as input 
        public ProducerController(IProducerService service)
        {
            _producerService = service; 
        }



        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            try
            {        
                var messages = await _producerService.GetMessagesAsync();
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
                return BadRequest("Invalid request body");
            }
          

            // Call the service method with the extracted values
            await _producerService.CreateMessageAsync(message);

            return Ok($"Message sent successfully with value: {message.Value} and counter: {message.Counter}");
        }



        [HttpPost("{number}")]
        public async Task<IActionResult> TriggerProducer(int number)
        {
            // Triggering the producer logic to send a message with post request 
            await _producerService.CreateMessagesAsync(number);

            return Ok("Message sent successfully");
        }


    }

    

