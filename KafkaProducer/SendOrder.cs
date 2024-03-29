using System.Net;
using Confluent.Kafka;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Orders
{
    public class SendOrder
    {
        private readonly ILogger _logger;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly string _topicName = Environment.GetEnvironmentVariable("Topic");

        public SendOrder(ILoggerFactory loggerFactory, IKafkaProducer kafkaProducer)
        {            
            _kafkaProducer = kafkaProducer;        
            _logger = loggerFactory.CreateLogger<SendOrder>();
        }

        [Function("SendOrder")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("C# HTTP trigger function invoked.");

            // Get the body of the request and send it to the Kafka topic
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();            
            await _kafkaProducer.SendEventAsync(_topicName, null, requestBody);
            
            return req.CreateResponse(HttpStatusCode.Created);          
        }
    }
}
