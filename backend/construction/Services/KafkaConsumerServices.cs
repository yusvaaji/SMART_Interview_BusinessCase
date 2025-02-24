using Confluent.Kafka;
using ConstructionProjectManagement.Models;
using Microsoft.Extensions.Hosting;
using Nest;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConstructionProjectManagement.Services
{ 
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly IElasticClient _elasticClient;

        public KafkaConsumerService(IConsumer<Null, string> consumer, IElasticClient elasticClient)
        {
            _consumer = consumer;
            _elasticClient = elasticClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("es.construction.hbx");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                if (consumeResult != null)
                {
                    var project = JsonSerializer.Deserialize<ConstructionProject>(consumeResult.Message.Value);
                    if (project != null)
                    {
                        if (!string.IsNullOrEmpty(project.ProjectName))
                        {
                            var indexResponse = await _elasticClient.IndexDocumentAsync(project);
                        }
                    } 
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}