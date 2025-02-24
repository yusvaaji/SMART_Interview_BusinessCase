using ConstructionProjectManagement.Models;
using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ConstructionProjectManagement.Services
{
    public class PostgresServices
    {
        private readonly string _connectionString;

        public PostgresServices(string connectionString)
        {
            _connectionString = connectionString;
        } 

        public async Task<IEnumerable<ConstructionProject>> GetAllProjectsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString); 
            await connection.OpenAsync();
            var query = "SELECT * FROM ConstructionProjects";
            return await connection.QueryAsync<ConstructionProject>(query);
        }
    }
}
