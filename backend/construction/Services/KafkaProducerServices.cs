using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ConstructionProjectManagement.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;
         
        public KafkaProducerService(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = configuration["Kafka:SaslUsername"],
                SaslPassword = configuration["Kafka:SaslPassword"],
                SslCaLocation = configuration["Kafka:SslCaLocation"] 
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topic = configuration["Kafka:Topic"];
        }

        public async Task ProduceAsync(string message)
        {
            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
        }
    }
}