using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
namespace PapenChat.Framework.Database
{
    public class GenericRepository<T> where T : class
    {
        private readonly string _tableName;

        public GenericRepository(string tableName)
        {
            _tableName = tableName;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection = DatabaseHelper.GetConnection();
            return await connection.QueryAsync<T>($"SELECT * FROM {_tableName}");
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using var connection = DatabaseHelper.GetConnection();
            return await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> InsertAsync(T entity)
        {
            using var connection = DatabaseHelper.GetConnection();
            var query = GenerateInsertQuery();
            return await connection.ExecuteAsync(query, entity);
        }

        public async Task<int> UpdateAsync(T entity)
        {
            using var connection = DatabaseHelper.GetConnection();
            var query = GenerateUpdateQuery();
            return await connection.ExecuteAsync(query, entity);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = DatabaseHelper.GetConnection();
            return await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id = @Id", new { Id = id });
        }

        private string GenerateInsertQuery()
        {
            var properties = typeof(T).GetProperties().Where(p => p.Name != "Id");
            var columnNames = string.Join(", ", properties.Select(p => p.Name));
            var paramNames = string.Join(", ", properties.Select(p => "@" + p.Name));

            return $"INSERT INTO {_tableName} ({columnNames}) VALUES ({paramNames})";
        }

        private string GenerateUpdateQuery()
        {
            var properties = typeof(T).GetProperties().Where(p => p.Name != "Id");
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

            return $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
        }
    }
}