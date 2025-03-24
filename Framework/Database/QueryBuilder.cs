using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace PapenChat.Framework.Database
{
    public class QueryBuilder<T> where T : class, new()
    {
        private readonly List<string> _conditions = new();
        private readonly Dictionary<string, object> _parameters = new();
        private string _orderBy = "";
        private readonly string _tableName = typeof(T).Name;

        public QueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            var condition = ExpressionToSql(predicate);
            _conditions.Add(condition.Item1);
            foreach (var param in condition.Item2)
            {
                _parameters.Add(param.Key, param.Value);
            }
            return this;
        }

        public QueryBuilder<T> OrderBy(Expression<Func<T, object>> columnSelector, bool descending = false)
        {
            var columnName = GetMemberName(columnSelector.Body);
            _orderBy = $"ORDER BY {columnName} " + (descending ? "DESC" : "ASC");
            return this;
        }

        public async Task<IEnumerable<T>> All()
        {
            using var connection = DatabaseHelper.GetConnection();
            var whereClause = _conditions.Any() ? "WHERE " + string.Join(" AND ", _conditions) : "";
            var sql = $"SELECT * FROM `{_tableName}` {whereClause} {_orderBy}";
            return await connection.QueryAsync<T>(sql, _parameters);
        }

        public async Task<T> First()
        {
            using var connection = DatabaseHelper.GetConnection();
            var whereClause = _conditions.Any() ? "WHERE " + string.Join(" AND ", _conditions) : "";
            var sql = $"SELECT * FROM `{_tableName}` {whereClause} {_orderBy} LIMIT 1";
            return await connection.QuerySingleOrDefaultAsync<T>(sql, _parameters);
        }


        private static (string, Dictionary<string, object>) ExpressionToSql(Expression<Func<T, bool>> expression)
        {
            var visitor = new SqlExpressionVisitor();
            visitor.Visit(expression.Body);
            return (visitor.Sql, visitor.Parameters);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression is MemberExpression member)
                return member.Member.Name;
            if (expression is UnaryExpression unary)
                return ((MemberExpression)unary.Operand).Member.Name;
            throw new ArgumentException("Invalid column selector expression");
        }
    }
}