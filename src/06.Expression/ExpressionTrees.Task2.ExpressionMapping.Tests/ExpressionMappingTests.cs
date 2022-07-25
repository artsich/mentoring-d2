using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
    public static class ExpressionHelper
    {
        public static PropertyInfo Property<S, D>(Expression<Func<S, D>> expr)
        {
            if (expr.Body is not MemberExpression memberExpr)
            {
                throw new InvalidOperationException($"Invalid expression {expr.Body}, only members supported");
            }

            if (memberExpr.Member is not PropertyInfo propertyInfo)
            {
                throw new InvalidOperationException($"Invalid expression {expr.Body}, only properties supported");
            }

            return propertyInfo;
        }
    }

    public interface ITypeConverter<S, D>
    {
        D From(S source);
    }

    public class StringToInt : ITypeConverter<string, int>
    {
        public int From(string source)
        {
            return int.Parse(source);
        }
    }
    
    public class MapperBuilder<TSource, TDestination>
    {
        private static readonly Type SourceType = typeof(TSource);
        private static readonly Type DestType = typeof(TDestination);

        private static ParameterExpression SourceParamExpr;
        private static ParameterExpression DestParamExpr;
        private static BinaryExpression NewDestExpr;

        static MapperBuilder()
        {
            SourceParamExpr = Expression.Parameter(SourceType, "source");
            DestParamExpr = Expression.Parameter(DestType, "dest");
            NewDestExpr = Expression.Assign(DestParamExpr, Expression.New(DestType));
        }
        
        private IList<Expression> _assignStatements = new List<Expression>();

        public MapperBuilder<TSource, TDestination> ForMember<T>(Expression<Func<TSource, T>> source,
            Expression<Func<TDestination, T>> dest)
        {
            var sourceProp = ExpressionHelper.Property(source);
            var destProp = ExpressionHelper.Property(dest);

            var sourceMember = Expression.PropertyOrField(SourceParamExpr, sourceProp.Name);
            var destMember = Expression.PropertyOrField(DestParamExpr, destProp.Name);
            var assignExpr = Expression.Assign(destMember, sourceMember);

            _assignStatements.Add(assignExpr);

            return this;
        }

        // public MapperBuilder<TSource, TDestination> ForMember<TSourceProp, TDestProp>(Expression<Func<TSource,T>> source,
        //     Expression<Func<TDestination, TDestProp>> dest, 
        //     ITypeConverter<TSourceProp, TDestProp> converter)
        // {
        //     throw new NotImplementedException();
        // }

        public Mapper<TSource, TDestination> Build()
        {
            var statements = new List<Expression>();
            statements.Add(NewDestExpr);

            statements.AddRange(_assignStatements);
            _assignStatements.Clear();

            ConfigureReturnExpr(DestParamExpr, statements);
            
            var blockExpr = Expression.Block(
                new[] {DestParamExpr}, 
                statements);

            var mapFunction =
                Expression.Lambda<Func<TSource, TDestination>>(
                    blockExpr,
                    SourceParamExpr);

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }

        private static void ConfigureReturnExpr(ParameterExpression destParamExpr, IList<Expression> statements)
        {
            var returnTarget = Expression.Label(DestType);
            var returnExpression = Expression.Return(
                returnTarget,
                destParamExpr,
                DestType);

            var returnLabel = Expression.Label(
                returnTarget,
                Expression.Constant(null, DestType));
            
            statements.Add(returnExpression);
            statements.Add(returnLabel);
        }
    }

    [TestClass]
    public class ExpressionMappingTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mapper = new MapperBuilder<Foo, Bar>()
                .ForMember(source => source.Id, dest => dest.Id)
                .ForMember(source => source.Name, dest => dest.Name)
                .Build();

            var foo = new Foo()
            {
                Id = 1,
                Name = "Name"
            };
            
            var res = mapper.Map(foo);
            //var res = new MappingGenerator().Generate<Foo, Bar>().Map(foo);
            
            Assert.AreEqual(foo.Id, res.Id);
            Assert.AreEqual(foo.Name, res.Name);
        }
    }
}
