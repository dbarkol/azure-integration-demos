using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using Orders;

public static class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration(configuration =>
            {
                var config = configuration.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

                var builtConfig = config.Build();
            })
            .ConfigureServices(services =>
            {
                var connectionString = Environment.GetEnvironmentVariable("EventHubConnectionString");
                var brokerList = Environment.GetEnvironmentVariable("BrokerList");                
                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(brokerList))
                {
                    throw new Exception("EventHubConnectionString or BrokerList is null");
                }

                var config = new ProducerConfig { 
                    BootstrapServers = brokerList,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = "$ConnectionString",
                    SaslPassword = connectionString
                };

                // Add Kafka producer to DI container
                services.AddSingleton<IKafkaProducer>(new KafkaProducer(brokerList, connectionString));           
            })
            .Build();

        host.Run();
    }
}
