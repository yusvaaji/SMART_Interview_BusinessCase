using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Text.Json;

namespace ConstructionProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructionProjectsController : ControllerBase
    {
        private readonly ConstructionDbContext _context;
        private readonly KafkaProducerService _kafkaProducer;
        private readonly IElasticClient _elasticClient;
        private readonly ElasticSearchService _elasticSearchService;

        public ConstructionProjectsController(ConstructionDbContext context, KafkaProducerService kafkaProducer, IElasticClient elasticClient, ElasticSearchService elasticSearchService)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
            _elasticClient = elasticClient;
            _elasticSearchService = elasticSearchService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ConstructionProject>>> GetProjects()
        {
            var projects = await _context.ConstructionProjects.ToListAsync();
            await _elasticSearchService.IndexProjectsAsync(projects);

            await _elasticSearchService.EnsureIndexExistsAsync("constructionprojects");

            var searchResponse = await _elasticClient.SearchAsync<ConstructionProject>(s => s.Index("constructionprojects").Query(q => q.MatchAll()));
            return Ok(searchResponse.Documents);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ConstructionProject>> GetProject(string id)
        {
            var project = await _elasticClient.GetAsync<ConstructionProject>(id, idx => idx.Index("constructionprojects"));
            if (!project.Found) return NotFound();
            return Ok(project.Source);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ConstructionProject>> CreateProject(ConstructionProject project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (project.ProjectStage == ProjectStage.Planning || 
                project.ProjectStage == ProjectStage.Design ||
                project.ProjectStage == ProjectStage.Construction)
            {
                if (project.ProjectConstructionStartDate <= DateTime.UtcNow)
                {
                    return BadRequest("Construction start date must be a future date.");
                }
            }
            DateTime localDateTime = project.ProjectConstructionStartDate;
            DateTime utcDateTime = localDateTime.ToUniversalTime();

            project.ProjectConstructionStartDate = utcDateTime;
            project.ProjectId = GenerateProjectId();
            await _context.ConstructionProjects.AddAsync(project);
            await _context.SaveChangesAsync();

            // Index to Elasticsearch
            var indexResponse = await _elasticClient.IndexDocumentAsync(project);
            if (!indexResponse.IsValid)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to index project in Elasticsearch");
            }

            var json = JsonSerializer.Serialize(project);
            await _kafkaProducer.ProduceAsync(json);

            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(string id, ConstructionProject project)
        {
            if (id != project.ProjectId) return BadRequest();

            DateTime localDateTime = project.ProjectConstructionStartDate;
            DateTime utcDateTime = localDateTime.ToUniversalTime();

            project.ProjectConstructionStartDate = utcDateTime;

            // Check if project exists
            var existingProject = await _context.ConstructionProjects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId);

            if (existingProject != null)
            {
                // Update existing project
                _context.Entry(project).State = EntityState.Modified;
            }
            else
            {
                // Create new project
                await _context.ConstructionProjects.AddAsync(project);
            }

            await _context.SaveChangesAsync();

            // Index to Elasticsearch
            var indexResponse = await _elasticClient.IndexDocumentAsync(project);
            if (!indexResponse.IsValid)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to index project in Elasticsearch");
            }

            var json = JsonSerializer.Serialize(project);
            await _kafkaProducer.ProduceAsync(json);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(string id)
        {
            //var project = await _context.ConstructionProjects.FindAsync(id);
            //if (project == null) return NotFound();

            //_context.ConstructionProjects.Remove(project);
            //await _context.SaveChangesAsync();

            // Delete from Elasticsearch
            var deleteResponse = await _elasticClient.DeleteAsync<ConstructionProject>(id, idx => idx.Index("constructionprojects"));
            if (!deleteResponse.IsValid)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete project from Elasticsearch");
            }

            var json = JsonSerializer.Serialize(new { Id = id, Deleted = true });
            await _kafkaProducer.ProduceAsync(json);

            return NoContent();
        }

        private string GenerateProjectId()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}