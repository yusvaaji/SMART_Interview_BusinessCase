using ConstructionProjectManagement.Models;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionProjectManagement.Services
{
    public class ElasticSearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        } 
         
        public async Task EnsureIndexExistsAsync(string indexName)
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName);
            if (!existsResponse.Exists)
            {
                var createIndexResponse = await _elasticClient.Indices.CreateAsync(indexName, c => c
                    .Map<ConstructionProject>(m => m.AutoMap())
                );

                if (!createIndexResponse.IsValid)
                {
                    throw new System.Exception($"Failed to create index: {createIndexResponse.ServerError}");
                }
            }
        }

        public async Task<BulkResponse> IndexProjectsAsync(IEnumerable<ConstructionProject> projects)
        {
            //var indexResponse = await _elasticClient.IndexManyAsync(projects, "constructionprojects");

            var bulkDescriptor = new BulkDescriptor();

            foreach (var project in projects)
            {
                bulkDescriptor.Index<ConstructionProject>(op => op
                    .Index("constructionprojects")
                    .Id(project.ProjectId)
                    .Document(project)
                );
            }

            var indexResponse = await _elasticClient.BulkAsync(bulkDescriptor);

            if (!indexResponse.IsValid)
            {
                // Log the error details
                Console.WriteLine($"Failed to index projects: {indexResponse.ServerError}");
                foreach (var itemWithError in indexResponse.ItemsWithErrors)
                {
                    Console.WriteLine($"Failed to index document {itemWithError.Id}: {itemWithError.Error}");
                }
            }
            return indexResponse;
            //if (!indexResponse.IsValid)
            //{
            //    // Log the error details
            //    Console.WriteLine($"Failed to index projects: {indexResponse.ServerError}");
            //    foreach (var itemWithError in indexResponse.ItemsWithErrors)
            //    {
            //        Console.WriteLine($"Failed to index document {itemWithError.Id}: {itemWithError.Error}");
            //    }
            //}
            //return indexResponse;
        }

        public async Task<ISearchResponse<ConstructionProject>> GetProjectsAsync()
        {
            return await _elasticClient.SearchAsync<ConstructionProject>(s => s
                .Index("constructionprojects")
                .Query(q => q.MatchAll())
            );
        }
    }
}