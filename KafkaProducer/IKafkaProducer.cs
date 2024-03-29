using System.Threading.Tasks;

namespace Orders
{
    public interface IKafkaProducer 
    {
        Task SendEventAsync(string topicName, string? key, string value);
    }
}