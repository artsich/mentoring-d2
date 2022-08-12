using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task2.ExpressionMapping;

public class Parameters
{
    private List<ParameterExpression> _parameters = new();

    public ParameterExpression Create(Type type, string name)
    {
        var expr = Expression.Parameter(type, name);
        _parameters.Add(expr);
        return expr;
    }

    public void Add(ParameterExpression parameter)
    {
        _parameters.Add(parameter);
    }

    public ICollection<ParameterExpression> PopAll()
    {
        var result = _parameters;
        _parameters = new();
        return result;
    }

    public IEnumerable<ParameterExpression> All() => _parameters;
}
