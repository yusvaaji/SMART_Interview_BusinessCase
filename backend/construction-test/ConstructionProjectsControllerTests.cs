using Xunit;
using Moq;
using ConstructionProjectManagement.Controllers;
using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConstructionProjectManagement.Tests
{
    public class ConstructionProjectsControllerTests
    {
        private readonly ConstructionDbContext _context;
        private readonly Mock<KafkaProducerService> _kafkaProducerMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly ConstructionProjectsController _controller;
        private readonly Mock<ElasticSearchService> _elasticSearchService;
        public ConstructionProjectsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConstructionDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ConstructionDbContext(options);

            var inMemorySettings = new Dictionary<string, string> {
      {"Kafka:BootstrapServers", "kafka-9644aa4-grafikyusvaaji-d9b0.k.aivencloud.com:19541"},
      {"Kafka:SecurityProtocol", "SASL_SSL"},
      {"Kafka:SaslMechanism", "PLAIN"},
      {"Kafka:SaslUsername", "your_username"},
      {"Kafka:SaslPassword", "your_password"},
      {"Kafka:SslCaLocation", "your_backend_path\\cert\\to\\ca-cert\\ca.pem"},
      {"Kafka:Topic", "es.construction.hbx"}
  };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _kafkaProducerMock = new Mock<KafkaProducerService>(MockBehavior.Strict, configuration);
            _elasticClientMock = new Mock<IElasticClient>();
            _elasticSearchService = new Mock<ElasticSearchService>();

            _controller = new ConstructionProjectsController(_context, _kafkaProducerMock.Object, _elasticClientMock.Object, _elasticSearchService.Object);
        }

        [Fact]
        public async Task CreateProject_ValidProject_ReturnsCreatedProject()
        {
            // Arrange
            var project = new ConstructionProject
            {
                ProjectName = "Project1",
                ProjectLocation = "Location1",
                ProjectStage = ProjectStage.Planning,
                ProjectCategory = ProjectCategory.Education,
                OtherCategory = "",
                ProjectConstructionStartDate = DateTime.UtcNow.AddMonths(1),
                ProjectDetails = "Details1",
                ProjectCreatorId = Guid.NewGuid()
            };

            // Act
            var result = await _controller.CreateProject(project) as ActionResult<ConstructionProject>;

            // Assert
            Assert.NotNull(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<ConstructionProject>(createdResult.Value);
            Assert.Equal(project.ProjectName, returnValue.ProjectName);
        }

        [Fact]
        public async Task CreateProject_InvalidConstructionStartDate_ReturnsBadRequest()
        {
            // Arrange
            var project = new ConstructionProject
            {
                ProjectName = "Project1",
                ProjectLocation = "Location1",
                ProjectStage = ProjectStage.Planning,
                ProjectCategory = ProjectCategory.Education,
                OtherCategory = "",
                ProjectConstructionStartDate = DateTime.UtcNow.AddDays(-1),
                ProjectDetails = "Details1",
                ProjectCreatorId = Guid.NewGuid()
            };

            // Act
            var result = await _controller.CreateProject(project);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateProject_MissingRequiredFields_ReturnsBadRequest()
        {
            // Arrange
            var project = new ConstructionProject
            {
                ProjectName = null,
                ProjectLocation = "Location1",
                ProjectStage = ProjectStage.Planning,
                ProjectCategory = ProjectCategory.Education,
                OtherCategory = "",
                ProjectConstructionStartDate = DateTime.UtcNow.AddMonths(1),
                ProjectDetails = "Details1",
                ProjectCreatorId = Guid.NewGuid()
            };

            // Act
            _controller.ModelState.AddModelError("ProjectName", "Required");
            var result = await _controller.CreateProject(project);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetProjects_ReturnsProjects()
        {
            // Arrange
            var projects = new List<ConstructionProject>
            {
                new ConstructionProject
                {
                    ProjectId = "000001",
                    ProjectName = "Project1",
                    ProjectLocation = "Location1",
                    ProjectStage = ProjectStage.Planning,
                    ProjectCategory = ProjectCategory.Education,
                    OtherCategory = "",
                    ProjectConstructionStartDate = DateTime.UtcNow.AddMonths(1),
                    ProjectDetails = "Details1",
                    ProjectCreatorId = Guid.NewGuid()
                }
            };

            var searchResponse = new Mock<ISearchResponse<ConstructionProject>>();
            searchResponse.Setup(x => x.Documents).Returns(projects);

            _elasticClientMock.Setup(x => x.SearchAsync<ConstructionProject>(It.IsAny<Func<SearchDescriptor<ConstructionProject>, ISearchRequest>>(), default))
                .ReturnsAsync(searchResponse.Object);

            // Act 
            var result = await _controller.GetProjects();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ConstructionProject>>(okResult.Value);
            Assert.Single(returnValue);
        }
        [Fact]
        public async Task UpdateProject_ValidProject_ReturnsNoContent()
        {
            // Arrange
            var project = new ConstructionProject
            {
                ProjectId = "000001",
                ProjectName = "Updated Project",
                ProjectLocation = "Updated Location",
                ProjectStage = ProjectStage.Planning,
                ProjectCategory = ProjectCategory.Education,
                ProjectConstructionStartDate = DateTime.UtcNow.AddMonths(1),
                ProjectDetails = "Updated Details",
                ProjectCreatorId = Guid.NewGuid()
            };

            _context.ConstructionProjects.Add(project);
            await _context.SaveChangesAsync();

            var updatedProject = new ConstructionProject
            {
                ProjectId = "000001",
                ProjectName = "Updated Project",
                ProjectLocation = "Updated Location",
                ProjectStage = ProjectStage.Planning,
                ProjectCategory = ProjectCategory.Education,
                ProjectConstructionStartDate = DateTime.UtcNow.AddMonths(2),
                ProjectDetails = "Updated Details",
                ProjectCreatorId = project.ProjectCreatorId
            };

            // Act
            var result = await _controller.UpdateProject("000001", updatedProject);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProject_ValidId_ReturnsNoContent()
        {
            // Arrange
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

            _context.ConstructionProjects.Add(project);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteProject("000001");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProject_InvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteProject("000999");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}