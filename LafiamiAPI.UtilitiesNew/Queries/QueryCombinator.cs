using System;
using System.Linq.Expressions;

namespace LafiamiAPI.Utilities.Queries
{
    public class QueryCombinator : ExpressionVisitor
    {
        private readonly Expression from, to;
        public QueryCombinator(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }
        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
        public static Expression<Func<T, bool>> MergeWithAnd<T>(
            Expression<Func<T, bool>> firstClause,
            Expression<Func<T, bool>> secondClause)
        {
            // rewrite e1, using the parameter from e2; "&&"
            Expression<Func<T, bool>> lambda1 = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(
                new QueryCombinator(firstClause.Parameters[0], secondClause.Parameters[0]).Visit(firstClause.Body),
                secondClause.Body), secondClause.Parameters);

            return lambda1;
        }

        public static Expression<Func<T, bool>> MergeWithOr<T>(Expression<Func<T, bool>> firstClause, Expression<Func<T, bool>> secondClause)
        {
            // rewrite e1, using the parameter from e2; "||"
            Expression<Func<T, bool>> lambda2 = Expression.Lambda<Func<T, bool>>(Expression.OrElse(
                new QueryCombinator(firstClause.Parameters[0], secondClause.Parameters[0]).Visit(firstClause.Body),
                secondClause.Body), secondClause.Parameters);

            return lambda2;
        }

    }
}
