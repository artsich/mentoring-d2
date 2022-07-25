using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees.Task2.ExpressionMapping;

[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public class MapperConfiguration<TSource, TDestination>
{
    private static readonly Type SourceType = typeof(TSource);
    private static readonly Type DestType = typeof(TDestination);
    private static readonly HashSet<string> SourceProperties;
    private static readonly HashSet<string> DestProperties;

    private static readonly ParameterExpression SourceParamExpr;
    private static readonly ParameterExpression DestParamExpr;

    static MapperConfiguration()
    {
        SourceParamExpr = Expression.Parameter(SourceType, "source");
        DestParamExpr = Expression.Parameter(DestType, "dest");

        SourceProperties = SourceType.GetProperties().Select(x => x.Name).ToHashSet();
        DestProperties = DestType.GetProperties().Select(x => x.Name).ToHashSet();
    }

    private readonly HashSet<string> _overriddenProperties = new();
    private readonly Parameters _parameters = new();
    private readonly Statements _statements = new();

    public MapperConfiguration()
    {
        ResetCollections();
    }

    public MapperConfiguration<TSource, TDestination> ForMember<T>(Expression<Func<TSource, T>> source,
        Expression<Func<TDestination, T>> dest)
    {
        var sourceProp = ExpressionHelper.PropertyInfo(source);
        if (!_overriddenProperties.Add(sourceProp.Name))
        {
            throw new InvalidOperationException($"Property: {sourceProp} already overridden");
        }

        var destProp = ExpressionHelper.PropertyInfo(dest);

        var sourceMember = Expression.PropertyOrField(SourceParamExpr, sourceProp.Name);
        var destMember = Expression.PropertyOrField(DestParamExpr, destProp.Name);

        _statements.CreateAssign(destMember, sourceMember);

        return this;
    }

    public MapperConfiguration<TSource, TDestination> ForMember<TSourceProp, TDestProp>(
        Expression<Func<TSource, TSourceProp>> source,
        Expression<Func<TDestination, TDestProp>> dest,
        ITypeConverter<TSourceProp, TDestProp> converter)
    {
        var sourceProp = ExpressionHelper.PropertyInfo(source);
        if (!_overriddenProperties.Add(sourceProp.Name))
        {
            throw new InvalidOperationException($"Property: {sourceProp} already overridden");
        }

        var destProp = ExpressionHelper.PropertyInfo(dest);
        var sourceMember = Expression.PropertyOrField(SourceParamExpr, sourceProp.Name);

        var converterType = typeof(ITypeConverter<TSourceProp, TDestProp>);
        var convertMethodInfo = converterType
            .GetMethod(nameof(ITypeConverter<TSourceProp, TDestProp>.From));

        var converterParameter = _parameters.Create(converterType, $"converter_{converter.GetType().Name}");

        _statements.CreateAssign(converterParameter, Expression.Constant(converter));
        _statements.CreateAssign(
            Expression.PropertyOrField(DestParamExpr, destProp.Name),
            Expression.Call(converterParameter, convertMethodInfo!, sourceMember));

        return this;
    }

    public Mapper<TSource, TDestination> Build()
    {
        AddDefaultMappings();

        _statements.CreateReturn<TDestination>(DestParamExpr);
        var blockExpression = _statements.ToBlock(_parameters);

        var mapFunction =
            Expression.Lambda<Func<TSource, TDestination>>(
                blockExpression, SourceParamExpr);

        ResetCollections();

        return new Mapper<TSource, TDestination>(mapFunction.Compile());
    }

    private void AddDefaultMappings()
    {
        var notOverriddenSource =
            SourceProperties
                .Where(name => !_overriddenProperties.Contains(name))
                .Where(name => DestProperties.Contains(name));

        foreach (var propertyName in notOverriddenSource)
        {
            _statements.CreateAssign(
                Expression.PropertyOrField(SourceParamExpr, propertyName),
                Expression.PropertyOrField(DestParamExpr, propertyName));
        }
    }

    private void ResetCollections()
    {
        _statements.PopAll();
        _parameters.PopAll();

        _statements.AddStatement(Expression.Assign(DestParamExpr, Expression.New(DestType)));
        _parameters.Add(DestParamExpr);
    }
}
