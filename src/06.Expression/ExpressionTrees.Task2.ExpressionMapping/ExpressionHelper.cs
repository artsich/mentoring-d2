using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping;

public static class ExpressionHelper
{
    public static PropertyInfo PropertyInfo<S, D>(Expression<Func<S, D>> expr)
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
