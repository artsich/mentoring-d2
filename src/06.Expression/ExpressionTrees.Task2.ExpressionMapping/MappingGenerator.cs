using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    public class MappingGenerator
    {
        public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
        {
            var sourceType = typeof(TSource);
            var destType = typeof(TDestination);

            var sourceParam = Expression.Parameter(typeof(TSource), "source");
            var destVar = Expression.Parameter(destType, "dest");

            var assignDestExpr = Expression.Assign(
                destVar,
                Expression.New(destType));

            var statements = new List<Expression>()
            {
                assignDestExpr
            };

            var destPropertiesMap = destType.GetProperties().ToDictionary(x => x.Name, x => x);
            foreach (var sourceProperty in sourceType.GetProperties())
            {
                if (!destPropertiesMap.TryGetValue(sourceProperty.Name, out var destProperty))
                {
                    continue;
                }

                var sourceMember = Expression.PropertyOrField(sourceParam, sourceProperty.Name);
                var destMember = Expression.PropertyOrField(destVar, destProperty.Name);

                var assignExpr = Expression.Assign(
                    destMember, sourceMember
                );
                statements.Add(assignExpr);
            }

            var returnTarget = Expression.Label(typeof(TDestination));
            var returnExpression = Expression.Return(
                returnTarget,
                destVar,
                typeof(TDestination));

            var returnLabel = Expression.Label(
                returnTarget,
                Expression.Constant(null, destType));

            statements.Add(returnExpression);
            statements.Add(returnLabel);

            var blockExpr = Expression.Block(new[] {destVar}, statements);

            var mapFunction =
                Expression.Lambda<Func<TSource, TDestination>>(
                    blockExpr,
                    sourceParam);

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }
    }
}
