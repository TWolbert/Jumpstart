namespace PapenChat.Framework.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    public class SqlExpressionVisitor : ExpressionVisitor
    {
        public string Sql { get; private set; } = "";
        public Dictionary<string, object> Parameters { get; } = new();
        private int _paramCounter = 0;

        protected override Expression VisitBinary(BinaryExpression node)
        {
            StringBuilder sql = new();

            sql.Append("(");
            Visit(node.Left);

            sql.Append(node.NodeType switch
            {
                ExpressionType.Equal => " = ",
                ExpressionType.NotEqual => " <> ",
                ExpressionType.GreaterThan => " > ",
                ExpressionType.GreaterThanOrEqual => " >= ",
                ExpressionType.LessThan => " < ",
                ExpressionType.LessThanOrEqual => " <= ",
                _ => throw new NotSupportedException($"Unsupported operator: {node.NodeType}")
            });

            Visit(node.Right);
            sql.Append(")");

            Sql = sql.ToString();
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression)
            {
                Sql += node.Member.Name;
            }
            else
            {
                object value = Expression.Lambda(node).Compile().DynamicInvoke();
                string paramName = $"@p{_paramCounter++}";
                Sql += paramName;
                Parameters[paramName] = value;
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            string paramName = $"@p{_paramCounter++}";
            Sql += paramName;
            Parameters[paramName] = node.Value;
            return node;
        }
    }

}