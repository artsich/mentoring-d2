using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task2.ExpressionMapping;

public class Statements
{
    private List<Expression> _statements = new();

    public Expression CreateAssign(Expression left, Expression right)
    {
        var result = Expression.Assign(left, right);
        _statements.Add(result);
        return result;
    }

    public void CreateReturn<TDestination>(Expression destParamExpr)
    {
        var destType = typeof(TDestination);

        var returnTarget = Expression.Label(destType);
        var returnExpression = Expression.Return(
            returnTarget,
            destParamExpr,
            destType);

        var defaultValue = Expression.Constant(null, destType);
        var returnLabel = Expression.Label(returnTarget, defaultValue);

        AddStatement(returnExpression);
        AddStatement(returnLabel);
    }

    public void AddStatement(Expression expression)
    {
        _statements.Add(expression);
    }

    public IEnumerable<Expression> PopAll()
    {
        var result = _statements;
        _statements = new List<Expression>();
        return result;
    }

    public BlockExpression ToBlock(Parameters parameters)
    {
        return Expression.Block(parameters.All(), _statements);
    }
}
