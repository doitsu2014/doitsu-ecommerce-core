using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Doitsu.Ecommerce.ApplicationCore.Utils
{
    public static class LinqExtensionUtils
    {
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Where(d => d.Count() > 0).Aggregate(
              emptyProduct,
              (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }
    }

    public class RemoveCastsVisitor : ExpressionVisitor
    {
        private static readonly ExpressionVisitor Default = new RemoveCastsVisitor();

        private RemoveCastsVisitor()
        {
        }

        public new static Expression Visit(Expression node)
        {
            return RemoveCastsVisitor.Default.Visit(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert && node.Type.IsAssignableFrom(node.Operand.Type))
            {
                return base.Visit(node.Operand);
            }
            return base.VisitUnary(node);
        }
    }

}
