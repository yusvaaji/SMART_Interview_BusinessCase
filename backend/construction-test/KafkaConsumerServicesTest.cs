using Xunit;
using Moq;
using Confluent.Kafka;
using ConstructionProjectManagement.Services;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using ConstructionProjectManagement.Models;
using System.Text.Json;

namespace ConstructionProjectManagement.Tests
{
    public class KafkaConsumerServiceTests
    {
        private readonly Mock<IConsumer<Null, string>> _consumerMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly KafkaConsumerService _consumerService;

        public KafkaConsumerServiceTests()
        {
            _consumerMock = new Mock<IConsumer<Null, string>>();
            _elasticClientMock = new Mock<IElasticClient>();
            _consumerService = new KafkaConsumerService(_consumerMock.Object, _elasticClientMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ConsumesAndIndexesMessages()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000); // Cancel after 2 seconds to stop the loop

            var project = new ConstructionProject
            {
                ProjectId = "000001",
                ProjectName = "Project1",
                ProjectLocation = "Location1",
                ProjectStage = ProjectStage.Planning,
                ProjectCategory = ProjectCategory.Education,
                ProjectConstructionStartDate = DateTime.UtcNow.AddMonths(1),
                ProjectDetails = "Details1",
                ProjectCreatorId = Guid.NewGuid() 
            };

            var consumeResult = new ConsumeResult<Null, string>
            {
                Message = new Message<Null, string> { Value = JsonSerializer.Serialize(project) }
            };

            _consumerMock.Setup(x => x.Consume(It.IsAny<CancellationToken>())).Returns(consumeResult);
            _elasticClientMock.Setup(x => x.IndexDocumentAsync(It.IsAny<ConstructionProject>(), It.IsAny<CancellationToken>())).ReturnsAsync(new IndexResponse());

            // Act
            await _consumerService.StartAsync(cts.Token);

            // Assert
            _consumerMock.Verify(x => x.Subscribe("es.construction.hbx"), Times.Once);
            _consumerMock.Verify(x => x.Consume(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _elasticClientMock.Verify(x => x.IndexDocumentAsync(It.IsAny<ConstructionProject>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public void Dispose_ClosesConsumer()
        {
            // Act
            _consumerService.Dispose();

            // Assert
            _consumerMock.Verify(x => x.Close(), Times.Once);
            _consumerMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}