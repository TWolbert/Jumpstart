using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PapenChat.Framework.Database
{
    public abstract class BaseModel<T> where T : class, new()
    {
        private static readonly GenericRepository<T> _repository = new(typeof(T).Name);
        private static readonly List<string> _conditions = new();
        private static readonly Dictionary<string, object> _parameters = new();

        public async Task<int> SaveAsync()
        {
            return await _repository.InsertAsync((T)(object)this);
        }

        public async Task<int> UpdateAsync()
        {
            return await _repository.UpdateAsync((T)(object)this);
        }

        public async Task<int> DeleteAsync()
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null) throw new Exception("Id property not found");

            var idValue = idProperty.GetValue(this);
            return await _repository.DeleteAsync((int)idValue);
        }

        public static async Task<IEnumerable<T>> AllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public static async Task<T> FindAsync(int id)
        {
            using var connection = DatabaseHelper.GetConnection();
            return await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM `{typeof(T).Name}` WHERE `Id` = @Id", new { Id = id });
        }


        public static QueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            return new QueryBuilder<T>().Where(predicate);
        }
    }

}
