using System.Threading.Tasks;
using Confluent.Kafka;

namespace Orders
{
    public class KafkaProducer : IKafkaProducer
    {                
        private IProducer<string, string> _producer;

        public KafkaProducer(string brokerList, 
            string connectionString)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connectionString                
            };

            _producer = new ProducerBuilder<string, string>(config).Build();            
        }

        public Task SendEventAsync(string topicName, string? key, string value)
        {
            // implement the SendEventAsync method
            return _producer.ProduceAsync(topicName, new Message<string, string>
            {
                Key = key,
                Value = value
            });
        }
    }
}