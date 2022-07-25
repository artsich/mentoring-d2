using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ExpressionTrees.Task2.ExpressionMapping;

internal static class ExpressionHelper
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
    
    public static Expression ExprMember<S, D>(ParameterExpression param, Expression<Func<S, D>> expr)
    {
        if (expr.Body.NodeType == ExpressionType.Parameter)
        {
            return param;
        }

        if (expr.Body is not MemberExpression memberExpr)
        {
            throw new InvalidOperationException($"Invalid expression {expr.Body}, only member access supported");
        }

        if (memberExpr.Member is not PropertyInfo propertyInfo)
        {
            throw new InvalidOperationException($"Invalid expression {expr.Body}, only properties supported");
        }

        return Expression.PropertyOrField(param, propertyInfo.Name);
    }
}
